using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.ViewModels.Respones
{
    public class TokenPriceCaching
    {
        public List<TokenPrice> TokenPrices { get; set; } = new List<TokenPrice>();
    }
    public class TokenPrice
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
