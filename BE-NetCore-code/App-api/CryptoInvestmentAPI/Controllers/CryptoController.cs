using CryptoCore.Services.Implements;
using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Requests;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualBasic;
using System.Security.Claims;
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

        [HttpGet]
        [Route("Tokens")]
        [Authorize]
        public async Task<IActionResult> GetAllTokenInfoAsync()
        {
            try
            {
                var cachedCryptoData = new CryptoTokenRespones();
                var cryptoDataCache = await _cache.GetAsync("cryptoDataCache");
                if (cryptoDataCache != null)
                {
                    cachedCryptoData = JsonSerializer.Deserialize<CryptoTokenRespones>(cryptoDataCache);
                    return Ok(cachedCryptoData);
                }
                var cryptoData = await _cryptoService.GetAllCryptoTokenAsync();
                if (cryptoData == null || !cryptoData.Any())
                {
                    return NotFound("No crypto data found");
                }
                // Chuyển đổi dữ liệu CryptoToken sang CryptoTokenRespone
                var cryptoDataRespone = cryptoData.Select(token => new CryptoTokenRespone
                {
                    Id = token.Id,
                    Name = token.Name,
                    Symbol = token.Symbol,
                    //IconUrl = token.IconUrl,
                    //CurrentPrice = token.CurrentPrice,
                    //HighestPrice = token.HighestPrice,
                    //DateCreate = token.DateCreate,
                    //IsActive = token.IsActive
                }).ToList();
                cachedCryptoData.CryptoTokens = cryptoDataRespone;
                cachedCryptoData.IsSuccess = true;
                cachedCryptoData.Message = "Crypto data fetched successfully";
                // Lưu dữ liệu vào cache
                await SetCryptoData(cachedCryptoData);
                return Ok(cachedCryptoData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching crypto data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("ExchangeToken")]
        [Authorize]
        public async Task<IActionResult> ExchangeTokenAsync([FromBody] ExchangeTokenRequest exchangeTokenRequest)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return BadRequest("User ID claim not found");
                }
                var result = await _cryptoService.ExchangeTokenAsync(userIdClaim.Value, exchangeTokenRequest);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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

        [HttpGet]
        [Route("UserTokens/{email}")]
        [Authorize]
        public async Task<IActionResult> GetTokensByUserEmail(string email)
        {
            try
            {
                var response = new UserTokenResponses();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return BadRequest("User ID claim not found");
                }
                var result = await _cryptoService.GetTokensByUserEmail(email);
                if (result == null || !result.Any())
                {
                    response.IsSuccess = true;
                    response.Message = "No tokens found for the user.";
                    return Ok(response);
                }
                response.IsSuccess = true;
                response.Message = "Tokens retrieved successfully.";
                response.Tokens = result;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task SetCryptoData(CryptoTokenRespones cryptoTokens)
        {
            var jsonString = JsonSerializer.Serialize(cryptoTokens);
            var cachedData = Encoding.UTF8.GetBytes(jsonString);
            var cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Sẽ hết hạn nếu không được truy cập trong 5 phút
                .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Hết hạn hoàn toàn sau 1 giờ

            await _cache.SetAsync("cryptoDataCache", cachedData, cacheOptions);
        }
    }
}
