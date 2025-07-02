using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.ViewModels.Respones
{
    public class AllUsersRespone
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "Actived";
        public string PhoneNumber { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public float Balance { get; set; } = 0;
        public float Profit { get; set; } = 0;

    }
}
