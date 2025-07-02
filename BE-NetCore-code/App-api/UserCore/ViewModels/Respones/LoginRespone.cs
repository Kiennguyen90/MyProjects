using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCore.ViewModels.Respones
{
    public class LoginRespone
    {
        public string Error { get; set; } = string.Empty;
        public UserInformationRespone UserInformation { get; set; } = new UserInformationRespone();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class UserInformationRespone
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
