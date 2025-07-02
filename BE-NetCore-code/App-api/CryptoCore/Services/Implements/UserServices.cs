using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Requests;
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
        public UserServices(
            CryptoDbcontext cryptoDbcontext
            , ILogger<UserServices> logger
            , IHttpClientFactory httpClientFactory)
        {
            _cryptoDbcontext = cryptoDbcontext;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("apiClient");
        }

        public async Task<bool> AddUserAsync(string groupId, string userName, string email)
        {
            try
            {
                var existingUser = await _cryptoDbcontext.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"User with email {email} already exists.");
                    return false;
                }
                
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId,
                    UserName = userName,
                    Email = email
                };
                var userManagementResponse = await IsUserManagementExists(email);
                if (userManagementResponse != null)
                {
                    user.Id = userManagementResponse.UserId;    
                }
                await _cryptoDbcontext.Users.AddAsync(user);
                var saveCount = await _cryptoDbcontext.SaveChangesAsync();
                return saveCount > 0;
            }
            catch (Exception e)
            {
                _logger.LogError("RegisterUser Error: " + e.Message);
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
