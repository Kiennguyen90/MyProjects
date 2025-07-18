using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.ViewModels.Respones
{
    public class UserInformationRespone
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "Inactive"; // Default status
        public string PhoneNumber { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public float Balance { get; set; } = 0;
        public float Profit { get; set; } = 0;
        public float TotalDeposit { get; set; } = 0;
        public float TotalWithdraw { get; set; } = 0;
        public string GroupId { get; set; } = string.Empty;
        public string GroupAdminId { get; set; } = string.Empty;
    }

    public class GetAllUserRespone
    {
        public List<UserInformationRespone> ListUser { get; set; } = new List<UserInformationRespone>();
        public string Message { get; set; } = string.Empty;
    }
}
