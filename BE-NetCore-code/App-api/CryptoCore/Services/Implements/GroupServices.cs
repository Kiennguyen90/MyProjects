using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.Services.Implements
{
    public class GroupServices : IGroupServices
    {
        private readonly CryptoDbcontext _cryptoDbcontext;
        private readonly ILogger<GroupServices> _logger;
        private readonly ICommonServices _commonServices;
        private readonly IUserServices _userServices;
        public GroupServices(
            CryptoDbcontext cryptoDbcontext
            , ILogger<GroupServices> logger
            , ICommonServices commonServices
            , IUserServices userServices)
        {
            _cryptoDbcontext = cryptoDbcontext;
            _logger = logger;
            _commonServices = commonServices;
            _userServices = userServices;
        }

        public async Task<Group> GetGroupIdByAdminIdAsync(string adminId)
        {
            try
            {
                var group = await _cryptoDbcontext.Groups.FirstOrDefaultAsync(x => x.AdminId == adminId);
                return group;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<GetAllUserRespone> GetAllUsersByAdminIdAsync(string adminId)
        {
            try
            {
                var response = new GetAllUserRespone()
                {
                    IsSuccess = true,
                    Message = "Get user information successfully",
                    ListUser = new List<UserInformationRespone>()
                };

                var group = await GetGroupIdByAdminIdAsync(adminId);
                if (group == null)
                {
                    _logger.LogWarning("No group found for admin ID: " + adminId);
                    response.IsSuccess = false;
                    response.Message = "No group found for the provided admin ID.";
                    return response;
                }
                var users = await _cryptoDbcontext.Users.Where(x => x.GroupId == group.Id).ToListAsync();
                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found for group ID: " + group.Id);
                    response.IsSuccess = false;
                    response.Message = "No users found in the group.";
                    return response;
                }
                var tokenPriceData = await _commonServices.GetTokenPriceDataFromCaching();
                if (tokenPriceData == null)
                {
                    _logger.LogWarning("Token price data is null or empty.");
                    response.IsSuccess = false;
                    response.Message = "Token price data is not available.";
                    return response;
                }

                foreach (var user in users)
                {
                    float profit = 0;
                    float totalPredictedAmount = await _userServices.GetTotalAmountByUserIdAsync(user.Id);
                    if (user.TotalDeposit - user.TotalWithdraw > 0)
                    {
                        profit = (user.Balance + totalPredictedAmount) / (user.TotalDeposit - user.TotalWithdraw) * 100 - 100;
                    }
                    else
                    {
                        if (user.TotalDeposit == 0)
                        {
                            profit = 0;
                        }
                        else
                        {
                            profit = (user.Balance + totalPredictedAmount + user.TotalWithdraw - user.TotalDeposit) / user.TotalDeposit * 100 - 100;
                        }
                    }
                    response.ListUser.Add(new UserInformationRespone
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Balance = user.Balance + totalPredictedAmount,
                        Profit = profit,
                        TotalDeposit = user.TotalDeposit,
                        TotalWithdraw = user.TotalWithdraw,
                        GroupId = user.GroupId,
                        Status = user.IsActive ? "Active" : "InActive"
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("GetUserByAdminId Error: " + e.Message);
                return null;
            }
        }

        public async Task<bool> RegisterGroup(string adminId)
        {
            try
            {
                var group = new Group()
                {
                    Id = Guid.NewGuid().ToString(),
                    AdminId = adminId
                };
                await _cryptoDbcontext.Groups.AddAsync(group);
                var saveCount = await _cryptoDbcontext.SaveChangesAsync();
                return saveCount > 0;
            }
            catch (Exception e)
            {
                _logger.LogError("RegisterGroup Error: " + e.Message);
                return false;
            }
        }

        public async Task<string> GetAdminIdByGroupIdAsync(string groupId)
        {
            try
            {
                var group = await _cryptoDbcontext.Groups.FindAsync(groupId);
                if (group == null)
                {
                    _logger.LogWarning("No group found for group ID: " + groupId);
                    return null;
                }
                return group.AdminId;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }
}
