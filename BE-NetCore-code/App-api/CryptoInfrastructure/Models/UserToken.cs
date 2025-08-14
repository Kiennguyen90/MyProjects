using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Models
{
    public class UserToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TokenId { get; set; } = string.Empty;
        public float CurrentAmount { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("TokenId")]
        public CryptoToken CryptoToken { get; set; }
    }
}
