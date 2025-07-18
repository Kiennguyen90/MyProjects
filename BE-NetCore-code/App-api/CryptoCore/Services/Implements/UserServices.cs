using Azure;
using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Requests;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

namespace CryptoCore.Services.Implements
{
    public class UserServices : IUserServices
    {
        private readonly CryptoDbcontext _cryptoDbcontext;
        private readonly ILogger<UserServices> _logger;
        private readonly HttpClient _httpClient;
        private readonly IGroupServices _groupServices;
        public UserServices(
            CryptoDbcontext cryptoDbcontext
            , ILogger<UserServices> logger
            , IHttpClientFactory httpClientFactory
            , IGroupServices groupServices)
        {
            _cryptoDbcontext = cryptoDbcontext;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("apiClient");
            _groupServices = groupServices;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var result = await _cryptoDbcontext.Users
                    .Include(x => x.Group)
                    .FirstOrDefaultAsync(x => x.Email == email);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving user by email {Email}", email);   
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            try
            {
                var result = await _cryptoDbcontext.Users.FindAsync(userId);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error retrieving user by id:{userId}");
                throw;
            }
        }
        public async Task<UserRespone> AddUserAsync(string adminId, string userName, string email, string phoneNumber)
        {
            try
            {
                var respone = new UserRespone();
                var group = await _groupServices.GetGroupIdByAdminIdAsync(adminId);
                if (group == null)
                {
                    _logger.LogWarning($"Group not found for admin ID {adminId}.");
                    respone.IsSuccess = false;
                    respone.Message = "Group not found.";
                    return respone;
                }
                var existingUser = await _cryptoDbcontext.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (existingUser != null && existingUser.GroupId != Constants.Id.DEFAULTGROUP)
                {
                    _logger.LogWarning($"User with email {email} already exists.");
                    respone.IsSuccess = false;
                    respone.Message = "User already exists in a different group.";
                    return respone;
                }
                else if (existingUser != null && existingUser.GroupId == Constants.Id.DEFAULTGROUP)
                {
                    existingUser.UserName = userName;
                    existingUser.Email = email;
                    existingUser.PhoneNumber = phoneNumber;
                    existingUser.GroupId = group.Id;
                    existingUser.LastUpdate = DateTime.UtcNow;
                    existingUser.UpdateBy = adminId;
                    _cryptoDbcontext.Users.Update(existingUser);
                }
                else if (existingUser == null)
                {
                    var user = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = group.Id,
                        UpdateBy = adminId,
                        UserName = userName,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        
                    };
                    var userManagementResponse = await IsUserManagementExists(email);
                    if (userManagementResponse != null && userManagementResponse.UserId != string.Empty)
                    {
                        user.Id = userManagementResponse.UserId;
                        user.IsActive = true;
                    }
                    await _cryptoDbcontext.Users.AddAsync(user);
                }
                var saveCount = await _cryptoDbcontext.SaveChangesAsync();
                if (saveCount > 0)
                {
                    respone.IsSuccess = true;
                    respone.Message = "User created successfully.";
                }
                else
                {
                    respone.IsSuccess = false;
                    respone.Message = "Failed to create user.";
                }
                return respone;
            }
            catch (Exception e)
            {
                var respone = new UserRespone();
                respone.IsSuccess = false;
                respone.Message = "Failed to create user.";
                _logger.LogError("RegisterUser Error: " + e.Message);
                return respone;
            }
        }

        public async Task<bool> UpdateUserInfoByAdminAsync(string adminId, string userId, UserInfoRequest userRequest)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if(user == null)
                {
                    _logger.LogError($"User with ID {userId} not found.");
                    return false;
                }
                var groupAdminId = await _groupServices.GetAdminIdByGroupIdAsync(user.GroupId);
                if (groupAdminId != adminId)
                {
                    _logger.LogWarning($"Admin ID {adminId} does not have permission to update user ID {userId}.");
                    return false;
                }
                user.UserName = userRequest.UserName;
                user.Email = userRequest.Email;
                user.PhoneNumber = userRequest.PhoneNumber;
                _cryptoDbcontext.Users.Update(user);
                var saveCount = await _cryptoDbcontext.SaveChangesAsync();
                return saveCount > 0;
            }
            catch (Exception e)
            {
                _logger.LogError($"Update UpdateUserInfoByAdminAsync error: {e}", e);
                return false;
            }
        }

        private async Task<UserManagementRequest> IsUserManagementExists(string email)
        {
            try
            {
                var serviceId = "7FF6451C-7D2E-4568-B6D2-D84E27E18319";
                HttpResponseMessage response = await _httpClient.GetAsync($"user-management/User/GetUserByEmail/{serviceId}/{email}");
                response.EnsureSuccessStatusCode();
                string jsonContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(jsonContent))
                {
                    _logger.LogWarning("No content received from User Management API.");
                    return null;
                }
                return JsonSerializer.Deserialize<UserManagementRequest>(jsonContent);
            }
            catch (Exception e)
            {
                _logger.LogError("IsUserExists Error: " + e.Message);
                return null;
            }
        }
    }
}
