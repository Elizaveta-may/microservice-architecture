using MedVisit.AuthServer.Models;
using MedVisit.AuthServer.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;

namespace MedVisit.AuthServer.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = await _authService.RegisterUserAsync(model);
                if (userId == null)
                {
                    return BadRequest(new { message = "Username or email already taken." });
                }

                try
                {
                    var httpClient = _httpClientFactory.CreateClient("PaymentService");

                    var response = await httpClient.PostAsJsonAsync("account", new { UserId=userId.Value });
                   
                    if (response.IsSuccessStatusCode)
                    {
                        return Ok(new { message = "User registered and payment account created successfully." });
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, new { message = "Failed to create payment account." });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "An error occurred while creating the payment account.", error = ex.Message });
                }
            }

            return BadRequest(ModelState);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await _authService.AuthenticateUserAsync(model.UserName, model.Password);
                return token != null ? Ok(new { token }) : Unauthorized(new { message = "Invalid username or password." });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("auth")]
        public IActionResult CheckAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "Token validation failed" });
            }

            HttpContext.Response.Headers.Add("x-user-id", userId);

            return Ok(new { message = "Token is valid", userId });
        }
    }
}
