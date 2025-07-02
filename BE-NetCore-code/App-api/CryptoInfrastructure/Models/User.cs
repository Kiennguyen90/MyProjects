using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        public ICollection<UserCryptoExchange> UserCryptoExchanges { get; set; } = new List<UserCryptoExchange>();
    }
}
