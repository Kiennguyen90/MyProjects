﻿using AutoMapper;
using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Respones;
using Constants = UserCore.Constants;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        ILogger<UserController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAplicationServices _aplicationServices;
        public UserController(UserManager<ApplicationUser> userManager, IMapper mapper, IAplicationServices aplicationServices, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _aplicationServices = aplicationServices;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // User was successfully created
                    return Ok("User created successfully");
                }

                // If we got this far, something failed, redisplay form
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                return BadRequest("Error creating user: " + e.Message);
            }

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(Constants.StatusCode.UserNotFound);
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                var userServices = await _aplicationServices.GetSevicesByUserIdAsync(id);
                var userInformationRespone = _mapper.Map<UserComonInfoRespone>(user);
                if (userRoles != null)
                {
                    userInformationRespone.userRole = userRoles.FirstOrDefault() ?? "";
                }
                if (userServices != null)
                {
                    var services = new List<UserServiceRespone>();
                    foreach (var userService in userServices)
                    {
                        var userServiceRespone = new UserServiceRespone
                        {
                            ServiceId = userService.ServiceId,
                            RoleId = userService.UserRoleService.RoleId,
                        };
                        services.Add(userServiceRespone);
                    }
                    userInformationRespone.Services = services;
                }
                return Ok(userInformationRespone);
            }
            catch (Exception e)
            {
                return BadRequest("Error retrieving user: " + e.Message);
            }
        }

        [HttpGet]
        [Route("GetUserByEmail/{serviceId}/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserByEmailAsync(string email, string serviceId)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null) {
                    var serviceTypeId = await _aplicationServices.GetServiceTypeIdByIdAsync(serviceId, "Free");
                    var isRegisterServiced = await _aplicationServices.RegisterServiceAsync(serviceId, user.Id, serviceTypeId);
                    if (!isRegisterServiced)
                    {
                        _logger.LogError($"Failed to register service {serviceId} for user {user.Id}");
                    }
                    var userInfo = new Dictionary<string, string>
                    {
                        {"UserId", user.Id ?? ""},
                        {"Email", user.Email ?? ""},
                        {"UserName", user.FullName ?? ""},
                    };
                    return Ok(JsonSerializer.Serialize(userInfo));
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest("Error retrieving user: " + e.Message);
            }
        }
    }
}
