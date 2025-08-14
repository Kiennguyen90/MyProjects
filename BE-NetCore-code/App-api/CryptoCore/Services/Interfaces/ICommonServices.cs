using CryptoCore.ViewModels.Respones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.Services.Interfaces
{
    public interface ICommonServices
    {
        Task<TokenPriceCaching> GetTokenPriceDataFromCaching();
    }
}
