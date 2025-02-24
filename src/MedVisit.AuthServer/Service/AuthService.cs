using MedVisit.AuthServer.Models;
using MedVisit.AuthServer.Service.Interfaces;
using MedVisit.Common.AuthDbContext;
using MedVisit.Common.AuthDbContext.Entities;
using MedVisit.Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MedVisit.Core.RabbitMq;

namespace MedVisit.AuthServer.Service
{
    public class AuthService : IAuthService 
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly EventPublisher _eventPublisher;

        public AuthService(AuthDbContext dbContext, IConfiguration configuration, EventPublisher eventPublisher)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _eventPublisher = eventPublisher;
        }

        public async Task<int?> RegisterUserAsync(RegisterModel model)
        {
            if (await _dbContext.Users.AnyAsync(u => u.UserName == model.UserName || u.Email == model.Email))
                return null;

            var (hash, salt) = PasswordService.HashPassword(model.Password);
            var user = new UserDb
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                PasswordHash = hash,
                PasswordSalt = salt, 
                Role = model.Role
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await SendNotificationAsync(user).ConfigureAwait(false);
            return user.Id; 
        }

        public async Task<string?> AuthenticateUserAsync(string userName, string password)
        {
            var user = await _dbContext.Users
                .AsNoTracking()  
                .SingleOrDefaultAsync(u => u.UserName == userName)
                .ConfigureAwait(false);

            if (user == null) return null; 

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

            var now = DateTime.UtcNow;
            var expirationTime = now.AddHours(1); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
                }),
                Expires = expirationTime,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                NotBefore = now, 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        private static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var salt = Convert.FromBase64String(storedSalt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            return Convert.ToBase64String(hash) == storedHash;
        }

        public async Task<string>  GenerateServiceToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "AuthServiceAccountId"),
                    new Claim(ClaimTypes.Role, "AuthServiceAccount")
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task SendNotificationAsync(UserDb user)
        {
            var message = $"Здравствуйте, {user.FirstName} {user.LastName}!\n" +
                  "Вы успешно зарегистрировались в сервисе онлайн-записи клиентов MedVisit.\n" +
                  "Теперь вы можете удобно записываться на услуги, управлять бронированиями и получать напоминания.";

            await _eventPublisher.PublishAsync("auth_exchange", new
            {
                UserId = user.Id,
                Status = "Success",
                Message = message,
                Subject = "Успешная регистрация в сервисе онлайн-записи MedVisit",
                Timestamp = DateTime.UtcNow
            }).ConfigureAwait(false);
        }
    }
}
