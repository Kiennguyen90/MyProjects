using CryptoCore.ViewModels.Requests;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.Services.Interfaces
{
    public interface IUserServices
    {
        Task<BaseRespone> AddUserAsync (string adminId, string userName, string email, string phoneNumber);
        Task<BaseRespone> UpdateUserBalanceAsync(string adminId, UserBalanceRequest userBalanceRequest);
        Task<User> GetUserByEmailAsync (string email);
        Task<User> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserInfoByAdminAsync(string adminId, string userId, UserInfoRequest userRequest);

        Task<List<UserToken>> GetTokensByUserIdAsync(string userId);
        Task<float> GetTotalAmountByUserIdAsync(string userId);
    }
}
