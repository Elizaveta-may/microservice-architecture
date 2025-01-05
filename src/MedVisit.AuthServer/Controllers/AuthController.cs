using MedVisit.AuthServer.Models;
using MedVisit.AuthServer.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedVisit.AuthServer.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserName = model.Email;
                var success = await _authService.RegisterUserAsync(model);
                return success ? Ok(new { message = "User registered successfully." }) : BadRequest(new { message = "Username or email already taken." });
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
    }
}