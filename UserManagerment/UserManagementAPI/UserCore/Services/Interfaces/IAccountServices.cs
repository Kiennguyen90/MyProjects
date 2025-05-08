using UserCore.ViewModels.Requests;
using UserCore.ViewModels.Respones;

namespace UserCore.Services.Interfaces
{
    public interface IAccountServices
    {
        Task<LoginRespone> RegisterAsync(RegisterRequest model);
        Task<LoginRespone> LoginAsync(string email, string password);
        Task<bool> LogoutAllDeviceAsync(string userId);

    }
}
