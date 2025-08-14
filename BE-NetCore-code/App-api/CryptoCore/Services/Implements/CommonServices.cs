using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Respones;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoCore.Services.Implements
{
    public class CommonServices : ICommonServices
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CommonServices> _logger;
        public CommonServices(IDistributedCache cache, ILogger<CommonServices> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public async Task<TokenPriceCaching> GetTokenPriceDataFromCaching()
        {
            try
            {
                var tokenPriceData = new TokenPriceCaching();
                var tokenPriceDataCaching = await _cache.GetAsync("tokenPriceCaching");
                if (tokenPriceDataCaching != null)
                {
                    tokenPriceData = JsonSerializer.Deserialize<TokenPriceCaching>(tokenPriceDataCaching);
                    if (tokenPriceData == null || tokenPriceData.TokenPrices == null || !tokenPriceData.TokenPrices.Any())
                    {
                        return null;
                    }
                    return tokenPriceData;
                }
                return tokenPriceData;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error retrieving token price caching data: {e.Message}");
                return null;
            }
        }
    }
}
