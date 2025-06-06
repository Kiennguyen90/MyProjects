using AutoMapper;
using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceBusDelivery;
using UserCore.Services.Implements;
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
                var isSuccess = await _aplicationServices.RegisterServiceAsync(request.ServiceId, request.UserId, request.TypeId);
                if (!isSuccess)
                {
                    return BadRequest(Constants.StatusCode.RegisterFailed);
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
            await _queue.SendMesssage("cryptoservice", "queue message");
            return Ok("test ok");
        }
    }
}
