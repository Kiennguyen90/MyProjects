using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.ViewModels.Requests
{
    public class ExchangeTokenRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string TokenId { get; set; } = string.Empty;
        public float TokenAmount { get; set; } = 0;
        public float Price { get; set; } = 0;
        public float AmountVnd { get; set; } = 0;
        public float AmountUsdt { get; set; } = 0;
        public bool IsBuy { get; set; } = true; // Default type is Buy
    }
}
