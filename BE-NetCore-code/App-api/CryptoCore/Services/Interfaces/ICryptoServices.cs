using CryptoCore.ViewModels.Requests;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure.Models;

namespace CryptoCore.Services.Interfaces
{
    public interface ICryptoServices
    {
        Task<List<CryptoToken>> GetAllCryptoTokenAsync();
        Task<BaseRespone> ExchangeTokenAsync(string adminId, ExchangeTokenRequest exchangeTokenRequest);
        Task<bool> GenegrateDefaultDataAsync();
        Task<List<TokenRespone>> GetTokensByUserEmail(string email);
    }
}
