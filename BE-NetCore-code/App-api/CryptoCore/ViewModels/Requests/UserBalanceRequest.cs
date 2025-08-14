using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.ViewModels.Requests
{
    public class UserBalanceRequest
    {
        public string UserId { get; set; } = string.Empty;
        public float Amount { get; set; } = 0;
        public bool IsDeposit { get; set; } = false;
    }
}
