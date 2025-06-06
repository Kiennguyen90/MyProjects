using AutoMapper;
using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Respones;
using Constants = UserCore.Constants;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAplicationServices _aplicationServices;
        public UserController(UserManager<ApplicationUser> userManager, IMapper mapper, IAplicationServices aplicationServices)
        {
            _userManager = userManager;
            _mapper = mapper;
            _aplicationServices = aplicationServices;
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
        public async Task<IActionResult> GetUser(string id)
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
                if(userServices != null)
                {
                    var services = new List <UserServiceRespone>();
                    foreach (var userService in userServices) {
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
    }
}
