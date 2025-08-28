using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Requests;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CryptoCore.Services.Implements
{
    public class UserServices : IUserServices
    {
        private readonly CryptoDbcontext _cryptoDbcontext;
        private readonly ILogger<UserServices> _logger;
        private readonly HttpClient _httpClient;
        
        private readonly ICommonServices _commonServices;
        public UserServices(
            CryptoDbcontext cryptoDbcontext
            , ILogger<UserServices> logger
            , IHttpClientFactory httpClientFactory
            , ICommonServices commonServices)
        {
            _cryptoDbcontext = cryptoDbcontext;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("apiClient");
            _commonServices = commonServices;
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
        public async Task<BaseRespone> AddUserAsync(string adminId, string userName, string email, string phoneNumber)
        {
            try
            {
                var respone = new BaseRespone();
                var group = await _cryptoDbcontext.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.AdminId == adminId);
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
                var respone = new BaseRespone();
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
                var groupAdmin = await _cryptoDbcontext.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.GroupId);
                if(groupAdmin == null)
                {
                    _logger.LogError($"Group for user ID {userId} not found.");
                    return false;
                }
                if (groupAdmin.Id != adminId)
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

        public async Task<BaseRespone> UpdateUserBalanceAsync(string adminId, UserBalanceRequest userBalanceRequest)
        {
            try
            {
                var respone = new BaseRespone()
                {
                    IsSuccess = true,
                    Message = string.Empty
                };
                var user = await GetUserByIdAsync(userBalanceRequest.UserId);
                if (user == null)
                {
                    respone.IsSuccess = false;
                    respone.Message = "User not found.";
                    return respone;
                }
                var group = await _cryptoDbcontext.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.GroupId);
                if (group == null)
                {
                    _logger.LogError($"Group for user ID {user.Id} not found.");
                    respone.IsSuccess = false;
                    respone.Message = "Group not found for user.";
                    return respone;
                }
                if (group.AdminId != adminId)
                {
                    respone.IsSuccess = false;
                    respone.Message = "No permission to update user balance.";
                    return respone;
                }
                if(userBalanceRequest.IsDeposit)
                {
                    user.Balance += userBalanceRequest.Amount;
                    user.TotalDeposit += userBalanceRequest.Amount;
                    _cryptoDbcontext.Users.Update(user);
                    var userBalanceAction = new UserBalanceAction()
                    {
                        UserId = user.Id,
                        Amount = userBalanceRequest.Amount,
                        IsDeposit = userBalanceRequest.IsDeposit,
                        CreatedBy = adminId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _cryptoDbcontext.UserBalanceActions.AddAsync(userBalanceAction);
                }
                else
                {
                    if (user.Balance < userBalanceRequest.Amount)
                    {
                        respone.IsSuccess = false;
                        respone.Message = "Insufficient balance.";
                        return respone;
                    }
                    user.Balance -= userBalanceRequest.Amount;
                    user.TotalWithdraw += userBalanceRequest.Amount;
                    _cryptoDbcontext.Users.Update(user);
                    var userBalanceAction = new UserBalanceAction()
                    {
                        UserId = user.Id,
                        Amount = userBalanceRequest.Amount,
                        IsDeposit = false,
                        CreatedBy = adminId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _cryptoDbcontext.UserBalanceActions.AddAsync(userBalanceAction);
                }
                var saveCount = await _cryptoDbcontext.SaveChangesAsync();
                respone.IsSuccess = saveCount > 0;
                respone.Message = "User balance updated successfully.";
                return respone;
            }
            catch (Exception e)
            {
                var respone = new BaseRespone();
                respone.IsSuccess = false;
                respone.Message = "Failed to update user balance.";
                _logger.LogError("UpdateUserBalance Error: " + e.Message);
                return respone;
            }
        }

        public async Task<List<UserToken>> GetTokensByUserIdAsync(string userId)
        {
            try
            {
                var tokens = _cryptoDbcontext.UserTokens
                    .Include(x => x.CryptoToken)
                    .Where(x => x.UserId == userId);
                if (tokens == null || !tokens.Any())
                {
                    return new List<UserToken>();
                }
                else
                {
                    return await tokens.ToListAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error retrieving tokens for user ID {userId}: {e.Message}");
                return new List<UserToken>();
            }
        }

        public async Task<float> GetTotalAmountByUserIdAsync(string userId)
        {
            try
            {
                float totalAmount = 0;
                var tokenPriceData = await _commonServices.GetTokenPriceDataFromCaching();
                if (tokenPriceData == null)
                {
                    _logger.LogWarning("Token price data is empty or not found in cache.");
                    return 0;
                }
                var tokens = await GetTokensByUserIdAsync(userId);
                if (tokens == null || !tokens.Any())
                {
                    return totalAmount;
                }

                foreach (var item in tokens)
                {
                    var tokenPrice = tokenPriceData.TokenPrices.FirstOrDefault(x => x.Symbol == item.CryptoToken.Symbol)?.Price;
                    if (tokenPrice == null)
                    {
                        tokenPrice = 0; // Default to 0 if price is not found
                    }
                    totalAmount += item.CurrentAmount * (float)tokenPrice * 26431;
                }
                return totalAmount;

            }
            catch (Exception e)
            {
                _logger.LogError($"Error retrieving tokens for user ID {userId}: {e.Message}");
                return 0;
            }
        }
    }
}
