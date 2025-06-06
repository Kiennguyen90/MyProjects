using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Models
{
    public class UserCryptoExchange
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string CryptoTokenId { get; set; } = string.Empty;
        public float AmountExchange { get; set; }
        public float PriceExchange { get; set; }
        public string Type { get; set; } = "Buy";
        public DateTime ExchangeDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CryptoTokenId")]
        public CryptoToken CryptoToken { get; set; }
    }
}
