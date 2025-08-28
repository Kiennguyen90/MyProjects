using Azure;
using Google.Apis.Auth;
using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Requests;
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var respone = await _accountServices.LoginAsync(model.Email, model.Password);
                    return Ok(respone);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest("Error logging in: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("LoginByRefreshtoken")]
        public async Task<IActionResult> ReGenerateAccessToken([FromBody] RegenerateAccessTokenRequest model)
        {
            try
            {
                var result = await _tokenServices.GeneratedAccessTokenbyRefreshToken(model.RefreshToken, model.Email);
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

        [HttpPost("auth/google")]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleTokenModel model)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.Token);
                var email = payload.Email;
                var name = payload.Name;

                var respone = await _accountServices.LoginByGoogleAsync(email);
                if (respone.Error == Constants.StatusCode.UserNotFound)
                {
                    RegisterRequest registerRequest = new RegisterRequest { Email = email, FullName = name, Password = "LoginByGoogle.240790", ConfirmPassword = "LoginByGoogle.240790" };
                    respone = await _accountServices.RegisterAsync(registerRequest);
                    return Ok(respone);
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
            catch (InvalidJwtException)
            {
                return Unauthorized();
            }
        }
    }
}
