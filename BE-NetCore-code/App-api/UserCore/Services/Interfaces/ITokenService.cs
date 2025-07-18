using CryptoInfrastructure.Model;
using UserCore.ViewModels.Respones;

namespace UserCore.Services.Interfaces
{
    public interface ITokenServices
    {
        Task<string> GenerateAccessToken(ApplicationUser user);
        Task<string> GenerateRefreshToken(ApplicationUser user);
        Task<RefreshAccesstokenRespone> GeneratedAccessTokenbyRefreshToken(string refreshToken, string email);
        Task<bool> ValidateAccessToken(string token, ApplicationUser user);
        Task<bool> RemoveTokens(string userId);
    }
}
