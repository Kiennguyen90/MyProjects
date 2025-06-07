using CryptoCore.Services.Implements;
using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoCore.BackgroundServices
{
    public class AzureServiceBusListener : BackgroundService
    {
        private readonly ILogger<AzureServiceBusListener> _logger;
        private readonly IServiceBusQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public AzureServiceBusListener (
            ILogger<AzureServiceBusListener> logger
            , IServiceBusQueue queue
            , IServiceScopeFactory scopeFactory
            )
        {
            _logger = logger;
            _queue = queue;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Listener Started");
           
            while (!stoppingToken.IsCancellationRequested) {
                var msg = await _queue.ReceiveMasssage("cryptoservice");
                if (!string.IsNullOrEmpty(msg))
                {
                    var userInfo = JsonSerializer.Deserialize<UserServiceRequest>(msg);
                    if (userInfo != null) {
                        var isRegisterUser = await RegisterUser(userInfo.UserId, userInfo.UserName, userInfo.Email);
                        var isRegisterGroup = false;
                        if (isRegisterUser && userInfo.ServiceRoleId == "0")
                        {
                            isRegisterGroup = await RegisterGroup(userInfo.UserId);
                        }
                        if (isRegisterGroup)
                        {
                            _logger.LogInformation("Receive cryptoservice bus succeed");
                        }
                        else 
                        {
                            _logger.LogInformation("Receive cryptoservice bus failed");
                        }
                    }
                }
            }
        }
        private async Task<bool> RegisterUser(string id, string userName, string email)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cryptoDbcontext = scope.ServiceProvider.GetRequiredService<CryptoDbcontext>();
                var user = new User()
                {
                    Id = id,
                    UserName = userName,
                    Email = email,
                    UserGroupId = "93F92BA0-1097-4AA1-832E-4818F8AAF48B"
                };
                await cryptoDbcontext.Users.AddAsync(user);
                var saveCount = await cryptoDbcontext.SaveChangesAsync();
                return saveCount > 0;
            }
            catch (Exception e)
            {
                _logger.LogError("RegisterUser Error: " + e.Message);
                return false;
            }
        }

        public async Task<bool> RegisterGroup(string adminId)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cryptoDbcontext = scope.ServiceProvider.GetRequiredService<CryptoDbcontext>();
                var group = new UserGroup()
                {
                    Id = Guid.NewGuid().ToString(),
                    AdminId = adminId
                };
                await cryptoDbcontext.UserGroups.AddAsync(group);
                var saveCount = await cryptoDbcontext.SaveChangesAsync();
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
