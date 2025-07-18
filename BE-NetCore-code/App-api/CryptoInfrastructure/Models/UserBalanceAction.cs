using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Models
{
    public class UserBalanceAction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public bool IsDeposit { get; set; } = false;
        public bool IsWithdraw { get; set; } = false;
        public float Amount { get; set; } = 0;
        public string UpdateBy { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("UserId")]
        public User User { get; set; } = new User();

    }
}
