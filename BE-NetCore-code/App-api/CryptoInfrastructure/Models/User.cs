using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public ICollection<UserCryptoExchange> UserCryptoExchanges { get; set; } = new List<UserCryptoExchange>();
    }
}
