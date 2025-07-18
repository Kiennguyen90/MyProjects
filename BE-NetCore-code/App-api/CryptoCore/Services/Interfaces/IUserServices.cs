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
        Task<UserRespone> AddUserAsync (string adminId, string userName, string email, string phoneNumber);
        Task<User> GetUserByEmailAsync (string email);
        Task<User> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserInfoByAdminAsync(string adminId, string userId, UserInfoRequest userRequest);
    }
}
