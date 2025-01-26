
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MedVisit.Core.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;
        IConfiguration _configuration;
        public AuthMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                var httpClient = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, _configuration["Endpoints:AuthService"]);
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var userId = response.Headers.Contains("x-user-id")
                    ? response.Headers.GetValues("x-user-id").FirstOrDefault()
                    : null;

                if (!string.IsNullOrEmpty(userId))
                {
                    var claims = new List<Claim> { new Claim("user_id", userId) };
                    var identity = new ClaimsIdentity(claims, "AuthService");
                    context.User = new ClaimsPrincipal(identity);
                }
            }
            await _next(context);
        }
    }

}
