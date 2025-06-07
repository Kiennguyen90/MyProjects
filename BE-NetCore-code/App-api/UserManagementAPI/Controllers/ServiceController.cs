using AutoMapper;
using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDelivery;
using System.Security.Claims;
using System.Text.Json;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Requests;
using Constants = UserCore.Constants;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAplicationServices _aplicationServices;
        private readonly IMapper _mapper;
        private readonly IServiceBusQueue _queue;

        public ServiceController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper, IAplicationServices aplicationServices,
            IServiceBusQueue queue)
        {
            _userManager = userManager;
            _mapper = mapper;
            _aplicationServices = aplicationServices;
            _queue = queue;
        }

        [HttpPost]
        [Authorize]
        [Route("RegisterService")]
        public async Task<IActionResult> RegisterService([FromBody] RegisterServiceRequest request)
        {
            try
            {
                var userClaims = User.Claims;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var emailClaim = User.FindFirst(ClaimTypes.Email);
                var userName = User.FindFirst("UserNameClaim");
                if (userIdClaim == null || emailClaim == null || userName == null) {
                    return BadRequest(Constants.StatusCode.RegisterFailed + "claim user infomation");
                }
                var isSuccess = await _aplicationServices.RegisterServiceAsync(request.ServiceId, userIdClaim.Value, request.TypeId);
                if (!isSuccess)
                {
                    return BadRequest(Constants.StatusCode.RegisterFailed);
                }
                if(request.ServiceId == Constants.Services.CRYPTO) 
                {
                    var messObj = new Dictionary<string, string>
                    {
                        {"UserId", userIdClaim.Value},
                        {"Email", emailClaim.Value},
                        {"UserName", userName.Value},
                        {"ServiceRole", request.TypeId.ToString()}
                    };
                    string jsonMess = JsonSerializer.Serialize(messObj);
                    await _queue.SendMesssage("cryptoservice", jsonMess);
                }
                return Ok(isSuccess);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.StatusCode.RegisterFailed + e.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetAllServices")]
        public async Task<IActionResult> GetAllServicesAsync()
        {
            try
            {
                var result = await _aplicationServices.GetAllSevicesAsync();
                if (result == null)
                {
                    return NotFound(Constants.StatusCode.GetServiceFailed);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.StatusCode.GetServiceFailed + e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetServicesByUserId")]
        public async Task<IActionResult> GetServicesByUserIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(Constants.StatusCode.UserNotFound);
                }
                var result = await _aplicationServices.GetSevicesByUserIdAsync(userId);
                if (result == null || result.Count == 0)
                {
                    return NotFound(Constants.StatusCode.GetServiceFailed);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.StatusCode.GetServiceFailed + e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{serviceId}")]
        public async Task<IActionResult> GetServiceByIdAsync(string serviceId)
        {
            try
            {
                var result = await _aplicationServices.GetSevicesByIdAsync(serviceId);
                if (result == null)
                {
                    return NotFound(Constants.StatusCode.GetServiceFailed);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.StatusCode.GetServiceFailed + e.Message);
            }
        }

        [HttpPost]
        [Route("Default")]
        public async Task<IActionResult> GenegrateDefaultDataAsync()
        {
            try
            {
                var result = await _aplicationServices.GenegrateDefaultData();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.StatusCode.GetServiceFailed + e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var messObj = new Dictionary<string, string>
            {
                {"UserId", "9ee58e17-e782-4fb6-a460-28cff3db106b"},
                {"Email", "kienit5@gmail.com" },
                {"UserName", "Kien Nguyen 4"},
                {"ServiceRoleId", "1"}
            };
            string jsonMess = JsonSerializer.Serialize(messObj);
            await _queue.SendMesssage("cryptoservice", jsonMess);
            return Ok("test ok");
        }
    }
}
