using Azure;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly ITokenServices _tokenServices;
        private readonly ILogger<AccountController> _logger;
        private readonly IAplicationServices _aplicationServices;
        private readonly IConfiguration _configuration;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AccountController> logger,
            ITokenServices tokenServices,
            IAplicationServices aplicationServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _tokenServices = tokenServices;
            _aplicationServices = aplicationServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        FullName = model.FullName,
                        UserName = model.Email,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var registerService = await _aplicationServices.RegisterServiceAsync(Constants.Services.CRYPTO, user.Id);
                        var registerRole = await _userManager.AddToRoleAsync(user, Constants.UserRoles.USER);
                        var respone = new LoginRespone();
                        var signIn = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
                        if (signIn.Succeeded && registerRole.Succeeded && registerService)
                        {
                            respone.UserId = user.Id;
                            respone.AccessToken = await _tokenServices.GenerateAccessToken(user);
                            respone.RefreshToken = await _tokenServices.GenerateRefreshToken(user);
                            return Ok(respone);
                        }
                        return Problem("Register Error");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
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
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var respone = new LoginRespone();
                    if (user == null)
                    {
                        return BadRequest(Constants.StatusCode.InvalidCredentials);
                    }

                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (!result.Succeeded)
                    {
                        return Unauthorized();
                    }
                    respone.UserId = user.Id;
                    respone.AccessToken = await _tokenServices.GenerateAccessToken(user);
                    respone.RefreshToken = await _tokenServices.GenerateRefreshToken(user);
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
                return Ok("Logout successful");
            }
            catch (Exception ex)
            {
                return BadRequest("Error logging out: " + ex.Message);
            }
        }

        [HttpPost("logoutall")]
        public async Task<IActionResult> LogoutAllAsync([FromBody] string userId)
        {
            try
            {
                await _signInManager.SignOutAsync();
                var result = await _tokenServices.RemoveTokens(userId ?? "");
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
