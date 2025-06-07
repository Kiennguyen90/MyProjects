using CryptoInfrastructure.Models;

namespace CryptoCore.Services.Interfaces
{
    public interface ICryptoServices
    {
        Task<List<CryptoToken>> GetAllCryptoTokenAsync();
    }
}
