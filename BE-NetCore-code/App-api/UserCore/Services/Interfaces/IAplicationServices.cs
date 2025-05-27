using Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCore.ViewModels.Respones;

namespace UserCore.Services.Interfaces
{
    public interface IAplicationServices
    {
        Task <bool> RegisterServiceAsync(string serviceId, string userId);
        Task<List<ServiceRespone>> GetAllSevicesAsync();
        Task<List<Service>> GetSevicesByUserIdAsync(string userId);

    }
}
