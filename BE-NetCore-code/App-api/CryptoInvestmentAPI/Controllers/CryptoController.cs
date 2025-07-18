using CryptoCore.Services.Implements;
using CryptoCore.Services.Interfaces;
using CryptoInfrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualBasic;
using System.Text;
using System.Text.Json;

namespace CryptoInvestmentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoController : Controller
    {
        private readonly ILogger<CryptoController> _logger;
        private readonly ICryptoServices _cryptoService;
        private readonly IDistributedCache _cache;
        public CryptoController(ILogger<CryptoController> logger, ICryptoServices cryptoService, IDistributedCache cache)
        {
            _logger = logger;
            _cryptoService = cryptoService;
            _cache = cache;
        }

        [HttpGet("GetCryptoData")]
        [Authorize]
        public async Task<IActionResult> GetAllTokenInfoAsync()
        {
            try
            {
                var cryptoDataCache = await _cache.GetAsync("cryptoDataCache");
                if (cryptoDataCache != null)
                {
                    var cachedCryptoData = JsonSerializer.Deserialize<List<CryptoToken>>(cryptoDataCache);
                    return Ok(cachedCryptoData);
                }
                var cryptoData = await _cryptoService.GetAllCryptoTokenAsync();
                if (cryptoData == null || !cryptoData.Any())
                {
                    return NotFound("No crypto data found");
                }
                // Lưu dữ liệu vào cache
                await SetCryptoData(cryptoData);
                return Ok(cryptoData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching crypto data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("GenegrateDefaultData")]
        public async Task<IActionResult> GenegrateDefaultDataAsync()
        {
            try
            {
                var result = await _cryptoService.GenegrateDefaultDataAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task SetCryptoData(List<CryptoToken> cryptoTokens)
        {
            var jsonString = JsonSerializer.Serialize(cryptoTokens);
            var cachedData = Encoding.UTF8.GetBytes(jsonString);
            var cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Sẽ hết hạn nếu không được truy cập trong 5 phút
                .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Hết hạn hoàn toàn sau 1 giờ

            await _cache.SetAsync("MyCachedData", cachedData, cacheOptions);
        }
    }
}
