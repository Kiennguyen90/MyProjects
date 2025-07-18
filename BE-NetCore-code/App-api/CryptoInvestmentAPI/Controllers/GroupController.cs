using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Respones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                var respone = new GetAllUserRespone();
                var userClaims = User.Claims;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    respone.Message = "No Permission";
                    return Ok(respone);
                }

                var users = await _groupServices.GetAllUsersByAdminIdAsync(userIdClaim.Value);
                if (users == null || !users.Any())
                {
                    respone.Message = "No Permission";
                    return Ok(respone);
                }

                users.ForEach(user =>
                {
                    float profit = 0;
                    
                    if (user.TotalDeposit - user.TotalWithdraw > 0)
                    {
                        profit = user.Balance / (user.TotalDeposit - user.TotalWithdraw) * 100;
                    }
                    else
                    {
                        if (user.TotalDeposit == 0)
                        {
                            profit = 0;
                        }
                        else
                        {
                            profit = (user.Balance + user.TotalWithdraw - user.TotalDeposit) / user.TotalDeposit * 100;
                        } 
                    }
                    respone.ListUser.Add(new UserInformationRespone
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Balance = user.Balance,
                        Profit = profit,
                        Status = user.IsActive ? "Active" : "Inactive"
                    });
                });

                respone.Message = "Get User succeed";
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
