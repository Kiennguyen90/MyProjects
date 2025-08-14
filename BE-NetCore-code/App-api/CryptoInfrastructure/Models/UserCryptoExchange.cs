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
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string TokenId { get; set; } = string.Empty;
        public float TokenAmount { get; set; }
        public float AmountVnd { get; set; }
        public float AmountUsdt { get; set; }
        public float Price { get; set; }
        public bool IsBuy { get; set; } = true;
        public string CreatedBy { get; set; } = "default"; // Default creator
        public DateTime ExchangeDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("TokenId")]
        public CryptoToken CryptoToken { get; set; }
    }
}
