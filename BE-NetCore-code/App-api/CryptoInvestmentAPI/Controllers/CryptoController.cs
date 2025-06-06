using CryptoCore.Services.Implements;
using CryptoCore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoInvestmentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoController : Controller
    {
        private readonly ILogger<CryptoController> _logger;
        private readonly ICryptoService _cryptoService;
        public CryptoController(ILogger<CryptoController> logger, ICryptoService cryptoService)
        {
            _logger = logger;
            _cryptoService = cryptoService;
        }

        [HttpGet("GetCryptoData")]
        [Authorize]
        public async Task<IActionResult> GetAllTokenInfoAsync()
        {
            try
            {
                var cryptoData = await _cryptoService.GetAllCryptoTokenAsync();
                return Ok(cryptoData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching crypto data");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
