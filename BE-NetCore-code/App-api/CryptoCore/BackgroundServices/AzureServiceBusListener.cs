using CryptoCore.ViewModels;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusDelivery;
using System.Text.Json;

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
                        var isRegisterGroup = false;
                        var isRegisterUser = false;
                        if (userInfo.ServiceRoleId == Constants.ServiceRoles.Pro)
                        {
                            var group = new UserGroup()
                            {
                                Id = Guid.NewGuid().ToString(),
                                AdminId = userInfo.UserId
                            };
                            isRegisterGroup = await RegisterGroup(group);
                            if (isRegisterGroup) {
                                isRegisterUser = await RegisterUser(userInfo.UserId, userInfo.UserName, userInfo.Email, group.Id);
                            }
                        }
                        else
                        {
                            isRegisterUser = await RegisterUser(userInfo.UserId, userInfo.UserName, userInfo.Email, Constants.Id.DEFAULTGROUP);
                        }
                        if (isRegisterUser)
                        {
                            _logger.LogInformation(Constants.StatusCodes.REGISTERUSERSUCCED);
                        }
                        else 
                        {
                            _logger.LogInformation(Constants.StatusCodes.REGISTERUSERFAILED);
                        }
                    }
                }
            }
        }
        private async Task<bool> RegisterUser(string id, string userName, string email, string groupId)
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

        public async Task<bool> RegisterGroup(UserGroup group)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cryptoDbcontext = scope.ServiceProvider.GetRequiredService<CryptoDbcontext>();
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
