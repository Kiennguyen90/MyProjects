using AutoMapper;
using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Requests;
using CryptoCore.ViewModels.Respones;
using CryptoInvestmentAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoInvestment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<CryptoController> _logger;
        private readonly IUserServices _userServices;
        private readonly IMapper _mapper;
        private readonly IGroupServices _groupServices;
        public UserController(ILogger<CryptoController> logger, IUserServices userServices, IGroupServices groupServices, IMapper mapper)
        {
            _logger = logger;
            _userServices = userServices;
            _groupServices = groupServices;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserAsync([FromBody] UserInfoRequest userInfoRequest)
        {
            try
            {
                var respone = new UserRespone();
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (adminIdClaim == null)
                {
                    respone.Message = "User ID claim not found";
                    respone.IsSuccess = false;
                    return Ok(respone);
                }
                
                respone = await _userServices.AddUserAsync(adminIdClaim.Value, userInfoRequest.UserName, userInfoRequest.Email, userInfoRequest.PhoneNumber);
                return Ok(respone);
            }
            catch (Exception e)
            {
                return BadRequest("Error creating user: " + e.Message);
            }
        }

        [HttpGet]
        [Route("{email}")]
        [Authorize]
        public async Task<IActionResult> GetUserInfoByEmailAsync(string email)
        {
            try
            {
                float profit = 0;
                var user = await _userServices.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("User not found");
                }

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
                var userInformationRespone = new UserInformationRespone() 
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Balance = user.Balance,
                    Profit = profit,
                    TotalDeposit = user.TotalDeposit,
                    TotalWithdraw = user.TotalWithdraw,
                    GroupId = user.GroupId,
                    GroupAdminId = user.Group?.AdminId ?? string.Empty,
                    Status = user.IsActive ? "Active" : "Inactive",
                };
                return Ok(userInformationRespone);
            }
            catch (Exception e)
            {
                return BadRequest("Error retrieving user: " + e.Message);
            }
        }

        [HttpPut]
        [Route("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfoByAdminAsync(string userId, [FromBody] UserInfoRequest userInfoRequest)
        {
            try
            {
                var respone = new UserRespone();
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (adminIdClaim == null)
                {
                    respone.Message = "No Permission";
                    respone.IsSuccess = false;
                    return Ok(respone);
                }
                respone.IsSuccess = await _userServices.UpdateUserInfoByAdminAsync(adminIdClaim.Value, userId, userInfoRequest);
                if (!respone.IsSuccess)
                {
                    respone.Message = "Error updating user information";
                    return Ok(respone);
                }
                respone.Message = "User information updated successfully";
                return Ok(respone);
            }
            catch (Exception e)
            {
                return BadRequest("Error retrieving user: " + e.Message);
            }
        }
    }
}
