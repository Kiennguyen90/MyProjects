

using CryptoInfrastructure.Models;

namespace CryptoCore.Services.Interfaces
{
    public interface IGroupServices
    {
        Task<bool> RegisterGroup(string adminId);
        Task<Group> GetGroupIdByAdminIdAsync(string adminId);
        Task<string> GetAdminIdByGroupIdAsync(string groupId);
        Task<List<User>> GetAllUsersByAdminIdAsync(string adminId);
    }
}
