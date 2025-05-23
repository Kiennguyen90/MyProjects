﻿using AutoMapper;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Respones;
using Constants = UserCore.Constants;

namespace UserManagementAPI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAplicationServices _aplicationServices;
        private readonly IMapper _mapper;

        public ServiceController(UserManager<ApplicationUser> userManager, IMapper mapper, IAplicationServices aplicationServices)
        {
            _userManager = userManager;
            _mapper = mapper;
            _aplicationServices = aplicationServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterService([FromBody] string servieceId, string userId)
        {
            try
            {
                var isSuccess = await _aplicationServices.RegisterServiceAsync(servieceId, userId);
                if (!isSuccess)
                {
                    return BadRequest(Constants.StatusCode.RegisterFailed);
                }
                return Ok(Constants.StatusCode.Success);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.StatusCode.RegisterFailed + e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllServicesAsync()
        {
            try
            {
                var result = await _aplicationServices.GetAllSevicesAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.StatusCode.RegisterFailed + e.Message);
            }
        }
    }
}
