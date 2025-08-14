using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.ViewModels.Respones
{
    public class CryptoTokenRespones
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<CryptoTokenRespone> CryptoTokens { get; set; } = new List<CryptoTokenRespone>();
    }
    public class CryptoTokenRespone
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }
}
