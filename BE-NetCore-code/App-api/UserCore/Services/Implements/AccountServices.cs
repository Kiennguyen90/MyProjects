﻿using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Requests;
using UserCore.ViewModels.Respones;

namespace UserCore.Services.Implements
{
    public class AccountServices : IAccountServices
    {
        private readonly ITokenServices _tokenServices;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountServices> _logger;
        public AccountServices(ITokenServices tokenServices,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountServices> logger,
            IAplicationServices aplicationServices,
            RoleManager<IdentityRole> roleManager)
        {
            _tokenServices = tokenServices;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<LoginRespone> RegisterAsync(RegisterRequest model)
        {
            try
            {
                var respone = new LoginRespone();
                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.DefaultRoles.USERNAME);
                    respone = await LoginAsync(model.Email, model.Password);
                    return respone;
                }
                else
                {
                    return new LoginRespone
                    {
                        Error = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error registering user {Email}", model.Email);
                return new LoginRespone
                {
                    Error = e.Message
                };
            }
        }
        
        public async Task<LoginRespone> LoginAsync(string email, string password)
        {
            try
            {
                var respone = new LoginRespone();
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    respone.Error = Constants.StatusCode.UserNotFound;
                    return respone;
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                if (!result.Succeeded)
                {
                    respone.Error = Constants.StatusCode.WrongPassWord;
                    return respone;
                }
                var userInformation = new UserInformationRespone
                {
                    UserId = user.Id,
                    UserName = user.FullName ?? "",
                    Email = user.Email ?? "",
                };

                respone.UserInformation = userInformation;
                respone.AccessToken = await _tokenServices.GenerateAccessToken(user);
                respone.RefreshToken = await _tokenServices.GenerateRefreshToken(user);
                return respone;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error logging in user {Email}", email);
                return new LoginRespone
                {
                    Error = e.Message
                };
            }
        }

        public async Task<LoginRespone> LoginByGoogleAsync(string email)
        {
            try
            {
                var respone = new LoginRespone();
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    respone.Error = Constants.StatusCode.UserNotFound;
                    return respone;
                }
                var userInformation = new UserInformationRespone
                {
                    UserId = user.Id,
                    UserName = user.FullName ?? "",
                    Email = user.Email ?? "",
                };

                respone.UserInformation = userInformation;
                respone.AccessToken = await _tokenServices.GenerateAccessToken(user);
                respone.RefreshToken = await _tokenServices.GenerateRefreshToken(user);
                return respone;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error logging in user {Email}", email);
                return new LoginRespone
                {
                    Error = e.Message
                };
            }
            
        }

        //public async Task<LoginRespone> LoginByRefreshToken(string refreshToken, string email)
        //{
        //    try
        //    {
        //        var respone = new LoginRespone();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error logging in user {Email}", email);
        //        return new LoginRespone
        //        {
        //            Error = e.Message
        //        };
        //    }
        //}

        public async Task<bool> LogoutAllDeviceAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }
                await _signInManager.SignOutAsync();
                await _tokenServices.RemoveTokens(userId);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error logging out user {UserId}", userId);
                return false;
            }
        }
    }

    

}
