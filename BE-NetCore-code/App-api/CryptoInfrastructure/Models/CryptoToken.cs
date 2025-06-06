using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Models
{
    public class CryptoToken
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public float CurrentPrice { get; set; }
        public float HighestPrice { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<UserCryptoExchange> UserCryptoExchanges { get; set; } = new List<UserCryptoExchange>();
    }
}
