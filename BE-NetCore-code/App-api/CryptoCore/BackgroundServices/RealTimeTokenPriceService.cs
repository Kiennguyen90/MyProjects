using CryptoCore.ViewModels.Respones;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace CryptoCore.BackgroundServices
{
    public class RealTimeTokenPriceService : BackgroundService
    {
        private readonly ILogger<RealTimeTokenPriceService> _logger;
        private readonly IDistributedCache _cache;
        private static readonly HttpClient client = new HttpClient();
        public RealTimeTokenPriceService(ILogger<RealTimeTokenPriceService> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Real-time token price service started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                var tokenPriceCaching = new TokenPriceCaching()
                {
                    TokenPrices = new List<TokenPrice>()
                    {
                        new TokenPrice
                        {
                            Symbol = "BTC",
                            Price = await GetPriceAsync("BTCUSDT")
                        },
                        new TokenPrice
                        {
                            Symbol = "ETH",
                            Price = await GetPriceAsync("ETHUSDT")
                        },
                        new TokenPrice
                        {
                            Symbol = "BNB",
                            Price = await GetPriceAsync("BNBUSDT")
                        },
                        new TokenPrice
                        {
                            Symbol = "XRP",
                            Price = await GetPriceAsync("XRPUSDT")
                        },
                        new TokenPrice
                        {
                            Symbol = "SOL",
                            Price = await GetPriceAsync("SOLUSDT")
                        },
                        new TokenPrice
                        {
                            Symbol = "ADA",
                            Price = await GetPriceAsync("ADAUSDT")
                        },
                        new TokenPrice
                        {
                            Symbol = "LTC",
                            Price = await GetPriceAsync("LTCUSDT")
                        },
                    }
                };

                await SetTokenPriceData(tokenPriceCaching);

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Adjust the delay as needed
            }
            _logger.LogInformation("Real-time token price service stopped.");
        }

        private async Task SetTokenPriceData(TokenPriceCaching tokenPriceCaching)
        {
            var jsonString = JsonSerializer.Serialize(tokenPriceCaching);
            var cachedData = Encoding.UTF8.GetBytes(jsonString);
            var cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Sẽ hết hạn nếu không được truy cập trong 5 phút
                .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Hết hạn hoàn toàn sau 1 giờ

            await _cache.SetAsync("tokenPriceCaching", cachedData, cacheOptions);
        }

        private static async Task<decimal> GetPriceAsync(string symbol)
        {
            // Construct the API endpoint URL
            string url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol}";

            // Send the GET request
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Parse the JSON response
            string responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);
            decimal price = json.Value<decimal>("price");

            return price;
        }
    }
}
