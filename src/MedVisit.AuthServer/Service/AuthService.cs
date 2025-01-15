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

namespace MedVisit.AuthServer.Service
{
    public class AuthService : IAuthService 
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(AuthDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUserAsync(RegisterModel model)
        {
            if (await _dbContext.Users.AnyAsync(u => u.UserName == model.UserName || u.Email == model.Email))
                return false;

            var (hash, salt) = PasswordService.HashPassword(model.Password);
            var user = new UserDb
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                PasswordHash = hash,
                PasswordSalt = salt
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<string?> AuthenticateUserAsync(string userName, string password)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            if (user == null) return null;
            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt)) return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
        
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var salt = Convert.FromBase64String(storedSalt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            return Convert.ToBase64String(hash) == storedHash;
        }
    }
}
