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
    public class UserServices : IUserServices
    {
        private readonly CryptoDbcontext _cryptoDbcontext;
        private readonly ILogger<UserServices> _logger;
        public UserServices(
            CryptoDbcontext cryptoDbcontext
            , ILogger<UserServices> logger)
        {
            _cryptoDbcontext = cryptoDbcontext;
            _logger = logger;
        }

        public async Task<bool> RegisterUser(string id, string userName, string email)
        {
            try
            {
                var user = new User()
                {
                    Id = id,
                    UserName = userName,
                    Email = email
                };
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
    }
}
