using Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UserCore.ViewModels.Respones;

namespace UserCore.Services.Interfaces
{
    public interface IUserServices
    {
        public Task<BaseRespone> AvatarUploadAsync(IFormFile avatar, string userId);
        public Task<byte[]> GetAvatarAsync(string userId);
    }
}
