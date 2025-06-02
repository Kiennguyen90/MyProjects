using Infrastructure;
using Infrastructure.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Respones;

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

        public async Task<List<ServiceRespone>> GetAllSevicesAsync()
        {
            try
            {
                var result = new List<ServiceRespone>();
                result = await _userDbContext.Services
                    .Include(t => t.ServiceTypes)
                    .Select(x => new ServiceRespone
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ServiceTypes = x.ServiceTypes.Select(st => new ServiceTypeRespone
                        {
                            Id = st.Id,
                            Name = st.Name,
                            Description = st.Description,
                            Price = st.Price
                        }).ToList()
                    })
                    .AsNoTracking()
                    .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<List<UserService>> GetSevicesByUserIdAsync(string userId)
        {
            try
            {
                var userServices = await _userDbContext.UserServices
                    .Include(x => x.Service)
                    .Include(r => r.UserRoleService)
                    .Where(us => us.UserId == userId).ToListAsync();
                return userServices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> RegisterServiceAsync(string serviceId, string userId, int serviceTypeId)
        {
            try
            {
                var userService = new UserService()
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceId = serviceId,
                    UserId = userId,
                    ServiceTypeId = serviceTypeId,
                    CreatedDate = DateTime.UtcNow,
                    ExpireDate = DateTime.UtcNow.AddYears(1)
                };
                await _userDbContext.UserServices.AddAsync(userService);

                var serviceType = await _userDbContext.ServiceTypes.FindAsync(serviceTypeId);
                var roles = await _userDbContext.Roles.ToListAsync();
                var groupadminRole = roles.FirstOrDefault(x => x.Name == "groupadmin");
                var memberRole = roles.FirstOrDefault(x => x.Name == "groupadmin");
                if (roles != null && groupadminRole != null && memberRole != null && serviceType != null)
                {
                    var userRoleService = new UserRoleService()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserServiceId = userService.Id,
                        RoleId = serviceType.Name == "Pro" ? groupadminRole.Id : memberRole.Id
                    };
                    await _userDbContext.UserRoleServices.AddAsync(userRoleService);
                }

                var saveRecord = await _userDbContext.SaveChangesAsync();
                return saveRecord > 0 ? true : false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Registering Service {serviceId} for user {userId}: " + ex.Message);
                return false;
            }
            
        }

        public async Task<ServiceRespone> GetSevicesByIdAsync(string serviceId)
        {
            try
            {
                var service = await _userDbContext.Services
                    .Include(t => t.ServiceTypes)
                    .FirstAsync(x => x.Id == serviceId);
                if (service == null)
                {
                    _logger.LogWarning($"Service with ID {serviceId} not found.");
                    return null;
                }
                var result = new ServiceRespone
                {
                    Id = service.Id,
                    Name = service.Name,
                    ServiceTypes = service.ServiceTypes.Select(st => new ServiceTypeRespone
                    {
                        Id = st.Id,
                        Name = st.Name,
                        Description = st.Description,
                        Price = st.Price
                    }).ToList()
                };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> GenegrateDefaultData()
        {
            try
            {
                if (await _userDbContext.Services.AnyAsync())
                {
                    _logger.LogInformation("Default data already exists.");
                    return true;
                }
                var defaultServices = new List<Service>
                {
                    new Service
                    {
                        Id = "7FF6451C-7D2E-4568-B6D2-D84E27E18319",
                        Name = "Crypto",
                        ServiceTypes = new List<ServiceType>
                        {
                            new ServiceType
                            {
                                Name = "Free",
                                Description = "Basic type",
                                Price = 0
                            },
                            new ServiceType
                            {
                                Name = "Pro",
                                Description = "Pro type",
                                Price = 500000
                            }
                        }
                    },
                    new Service
                    {
                        Id = "B11CE3B0-3074-421C-A601-B7BF9252C78C",
                        Name = "Shop House",
                        ServiceTypes = new List<ServiceType>
                        {
                            new ServiceType
                            {
                                Name = "Free",
                                Description = "Basic type",
                                Price = 0
                            },
                            new ServiceType
                            {
                                Name = "Pro",
                                Description = "Pro type",
                                Price = 500000
                            }
                        }
                    },
                };
                var defaultRoles = new List<IdentityRole>
                {
                    new IdentityRole
                    {
                        Id = "42CD4109-6174-4FE0-A912-5AA0C1410A6A",
                        Name = "admin",
                        NormalizedName = "ADMIN"
                    },
                    new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "user",
                        NormalizedName = "USER"
                    },
                    new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "groupadmin",
                        NormalizedName = "GROUPADMIN"
                    },
                    new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "member",
                        NormalizedName = "MEMBER"
                    },
                };
                await _userDbContext.Services.AddRangeAsync(defaultServices);
                await _userDbContext.Roles.AddRangeAsync(defaultRoles);

                var saveRecord = await _userDbContext.SaveChangesAsync();

                return saveRecord > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating default data: {ex.Message}");
                return false;
            }
        }
    }
}
