using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Respones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CryptoInvestment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : Controller
    {
        private readonly ILogger<GroupController> _logger;
        private readonly IUserServices _userServices;
        private readonly IGroupServices _groupServices;
        public GroupController(ILogger<GroupController> logger, IUserServices userServices, IGroupServices groupServices)
        {
            _logger = logger;
            _userServices = userServices;
            _groupServices = groupServices;
        }

        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            try
            {
                Stopwatch sw1 = new Stopwatch();
                Stopwatch sw2 = new Stopwatch();
                sw1.Start();
                var respone = new GetAllUserRespone();
                var userClaims = User.Claims;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    respone.IsSuccess = false;
                    respone.Message = "No Permission";
                    return Ok(respone);
                }
                sw1.Stop();
                sw2.Start();
                respone = await _groupServices.GetAllUsersByAdminIdAsync(userIdClaim.Value);
                sw2.Stop();
                
                _logger.LogWarning("excute time: sw1:" + sw1.Elapsed.ToString() + "sw2: " + sw2.Elapsed.ToString());
                return Ok(respone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching groups");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("GetAdmin/{groupId}")]
        [Authorize]
        public async Task<IActionResult> GetAdminIdByGroupId(string groupId)
        {
            try
            {
                var adminId = await _groupServices.GetAdminIdByGroupIdAsync(groupId);
                return Ok(adminId);
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
