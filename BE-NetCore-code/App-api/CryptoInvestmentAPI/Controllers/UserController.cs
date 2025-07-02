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
        private readonly IGroupServices _groupServices;
        public UserController(ILogger<CryptoController> logger, IUserServices userServices, IGroupServices groupServices)
        {
            _logger = logger;
            _userServices = userServices;
            _groupServices = groupServices;
        }

        [HttpPost]
        [Route("AddUser")]
        [Authorize]
        public async Task<IActionResult> AddUserAsync([FromBody] UserRequest userRequest)
        {
            try
            {
                var respone = new AddUserRespone();
                var userClaims = User.Claims;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    respone.Message = "User ID claim not found";
                    respone.IsSuccess = false;
                    return Ok(respone);
                }
                var group = await _groupServices.GetGroupIdByAdminId(userIdClaim.Value);
                if (group == null)
                {
                    respone.Message = "User is not an admin of any group";
                    respone.IsSuccess = false;
                    return Ok(respone);
                }
                var result = await _userServices.AddUserAsync(group.Id, userRequest.UserName, userRequest.Email);
                if (result)
                {
                    respone.IsSuccess = true;
                    respone.Message = "User created successfully";
                    return Ok(respone);
                }
                respone.Message = "Error creating user";
                return Ok(respone);
            }
            catch (Exception e)
            {
                return BadRequest("Error creating user: " + e.Message);
            }
        }


    }
}
