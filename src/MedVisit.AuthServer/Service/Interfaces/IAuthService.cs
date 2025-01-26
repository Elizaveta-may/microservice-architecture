using MedVisit.AuthServer.Models;

namespace MedVisit.AuthServer.Service.Interfaces
{
    public interface IAuthService
    {
        Task<int?> RegisterUserAsync(RegisterModel model);
        Task<string?> AuthenticateUserAsync(string userName, string password);
        Task<string> GenerateServiceToken();
    }

}
