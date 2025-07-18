using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCore.ViewModels.Respones
{
    public class RefreshAccesstokenRespone
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
