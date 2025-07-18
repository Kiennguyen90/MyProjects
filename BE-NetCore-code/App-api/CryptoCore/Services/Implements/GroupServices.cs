using CryptoCore.Services.Interfaces;
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
        public GroupServices(
            CryptoDbcontext cryptoDbcontext
            ,ILogger<GroupServices> logger)
        {
            _cryptoDbcontext = cryptoDbcontext;
            _logger = logger;
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

        public async Task<List<User>> GetAllUsersByAdminIdAsync(string adminId)
        {
            try
            {
                var group = await GetGroupIdByAdminIdAsync(adminId);
                if (group == null)
                {
                    _logger.LogWarning("No group found for admin ID: " + adminId);
                    return new List<User>();
                }
                var users = await _cryptoDbcontext.Users.Where(x => x.GroupId == group.Id).ToListAsync();
                return users;
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
