using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.ViewModels.Respones
{
    public class UserTokenResponses : BaseRespone
    {
        public List<TokenRespone> Tokens{ get; set; } = new List<TokenRespone>();
    }
    public class TokenRespone
    {
        public string TokenId { get; set; } = string.Empty;
        public string TokenName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public float TotalTokenAmount { get; set; }
        public float TotalValue { get; set; }
        public float CurrentPrice { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
    
    public class Transaction
    {
        public float TokenAmount { get; set; }
        public float Price { get; set; }
        public float AmountVnd { get; set; }
        public float AmountUsdt { get; set; }
        public bool IsBuy { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ExchangeDate { get; set; }
    }
}
