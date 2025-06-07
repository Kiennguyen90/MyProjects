using CryptoCore.Services.Interfaces;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
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

        public async Task<bool> RegisterGroup(string adminId)
        {
            try
            {
                var group = new UserGroup()
                {
                    Id = Guid.NewGuid().ToString(),
                    AdminId = adminId
                };
                await _cryptoDbcontext.UserGroups.AddAsync(group);
                var saveCount = await _cryptoDbcontext.SaveChangesAsync();
                return saveCount > 0;
            }
            catch (Exception e)
            {
                _logger.LogError("RegisterGroup Error: " + e.Message);
                return false;
            }
        }
    }
}
