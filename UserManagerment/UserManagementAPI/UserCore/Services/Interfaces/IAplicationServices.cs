using Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCore.Services.Interfaces
{
    public interface IAplicationServices
    {
        Task <bool> RegisterServiceAsync(string serviceId, string userId);
        Task<List<ApplicationService>> GetAllSevicesAsync();
        Task<List<ApplicationService>> GetSevicesByUserIdAsync(string userId);

    }
}
