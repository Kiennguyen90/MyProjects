using CryptoInfrastructure.Models;

namespace CryptoCore.Services.Interfaces
{
    public interface ICryptoService
    {
        Task<List<CryptoToken>> GetAllCryptoTokenAsync();
    }
}
