using Infrastructure;
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCore.Services.Interfaces;

namespace UserCore.Services.Implements
{
    public class ApplicationServices : IAplicationServices  
    {
        private readonly UserDbContext _userDbContext;
        private readonly ILogger _logger;
        public ApplicationServices(UserDbContext userDbContext, ILogger<ApplicationServices> logger)
        {
            _userDbContext = userDbContext;
            _logger = logger;
        }

        public async Task<List<ApplicationService>> GetAllSevicesAsync()
        {
            try
            {
               return await _userDbContext.ApplicationServices.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<List<ApplicationService>> GetSevicesByUserIdAsync(string userId)
        {
            try
            {
                var userServices = await _userDbContext.ApplicationUserServices.Where(x=> x.UserId == userId).AsNoTracking().ToListAsync();
                if (userServices == null || userServices.Count == 0)
                {
                    return null;
                }
                var serviceIds = userServices.Select(x => x.ServiceId).ToList();
                var services = await _userDbContext.ApplicationServices
                    .Where(x => serviceIds.Contains(x.Id))
                    .AsNoTracking()
                    .ToListAsync();
                return services;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
           
        }

        public async Task<bool> RegisterServiceAsync(string serviceId, string userId)
        {
            try
            {
                await _userDbContext.ApplicationUserServices.AddAsync(new ApplicationUserService
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceId = serviceId,
                    UserId = userId
                });
                var saveRecord = await _userDbContext.SaveChangesAsync();
                return saveRecord > 0 ? true : false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Registering Service {serviceId} for user {userId}: " + ex.Message);
                return false;
            }
            
        }
    }
}
