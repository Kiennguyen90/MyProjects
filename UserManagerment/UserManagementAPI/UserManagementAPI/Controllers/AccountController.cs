using Azure;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Requests;
using UserCore.ViewModels.Respones;
using Constants = UserCore.Constants;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountServices _accountServices;
        private readonly ITokenServices _tokenServices;
        private readonly IAplicationServices _aplicationServices;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AccountController> logger,
            ITokenServices tokenServices,
            IAplicationServices aplicationServices,
            IAccountServices accountServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenServices = tokenServices;
            _aplicationServices = aplicationServices;
            _accountServices = accountServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _accountServices.RegisterAsync(model);
                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest("Error creating user: " + ex.Message);
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var respone = await _accountServices.LoginAsync(model.Email, model.Password);
                    if (respone.Error == Constants.StatusCode.UserNotFound)
                    {
                        return BadRequest(Constants.StatusCode.UserNotFound);
                    }
                    else if (respone.Error == Constants.StatusCode.WrongPassWord)
                    {
                        return BadRequest(Constants.StatusCode.WrongPassWord);
                    }
                    else if (respone.Error != string.Empty)
                    {
                        return BadRequest("Login failed: " + respone.Error);
                    }
                    return Ok(respone);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest("Error logging in: " + ex.Message);
            }

        }

        [HttpPost("updateaccesstoken")]
        public async Task<IActionResult> ReGenerateAccessToken([FromBody] RegenerateAccessTokenRequest model)
        {
            try
            {
                var result = await _tokenServices.GeneratedAccessTokenbyRefreshToken(model.RefreshToken, model.Email);
                if (result == Constants.StatusCode.UserNotFound)
                {
                    return BadRequest("User not found");
                }
                else if (result == Constants.StatusCode.RefreshTokenNotFound)
                {
                    return BadRequest("Refresh token not found");
                }
                else if (result == Constants.StatusCode.RefreshTokenExpired)
                {
                    return BadRequest("Refresh token expired");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Error generating access token: " + ex.Message);
            }

        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest("Error logging out: " + ex.Message);
            }
        }

        [HttpPost("logoutalldevice")]
        public async Task<IActionResult> LogoutAllAsync([FromBody] string userId)
        {
            try
            {
                var result = await _accountServices.LogoutAllDeviceAsync(userId);

                if (result)
                {
                    return Ok("Logout successful");
                }
                return BadRequest("Logout failed");
            }
            catch (Exception ex)
            {
                return BadRequest("Error logging out: " + ex.Message);
            }
        }
    }
}
