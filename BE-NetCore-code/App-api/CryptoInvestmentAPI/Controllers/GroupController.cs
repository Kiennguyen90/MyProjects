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
                var userClaims = User.Claims;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return BadRequest("User ID claim not found");
                }

                var users = await _groupServices.GetAllUsersByAdminId(userIdClaim.Value);
                if (users == null || !users.Any())
                {
                    return NotFound("No users found for this group");
                }
                var respone = users.Select(u => new AllUsersRespone
                {
                    UserId = u.Id,
                    Email = u.Email,
                    FullName = u.UserName,
                    PhoneNumber = u.PhoneNumber
                }).ToList();
                return Ok(respone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching groups");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
