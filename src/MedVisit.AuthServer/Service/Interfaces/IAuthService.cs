using MedVisit.AuthServer.Models;

namespace MedVisit.AuthServer.Service.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(RegisterModel model);
        Task<string?> AuthenticateUserAsync(string userName, string password);
    }

}
