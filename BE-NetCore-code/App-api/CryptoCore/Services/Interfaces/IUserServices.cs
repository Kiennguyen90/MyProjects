using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.Services.Interfaces
{
    public interface IUserServices
    {
        Task<bool> RegisterUser (string id, string userName, string email);
    }
}
