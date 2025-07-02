

using CryptoInfrastructure.Models;

namespace CryptoCore.Services.Interfaces
{
    public interface IGroupServices
    {
        Task<bool> RegisterGroup(string adminId);
        Task<Group> GetGroupIdByAdminId(string adminId);
        Task<List<User>> GetAllUsersByAdminId(string adminId);
    }
}
