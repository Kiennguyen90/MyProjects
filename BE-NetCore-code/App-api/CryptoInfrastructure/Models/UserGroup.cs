using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoInfrastructure.Models
{
    public class UserGroup
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string AdminId { get; set; } = string.Empty;

        [ForeignKey("AdminId")]
        public User Admin { get; set; }

        public List<User> Members { get; set; } = new List<User>();
    }
}
