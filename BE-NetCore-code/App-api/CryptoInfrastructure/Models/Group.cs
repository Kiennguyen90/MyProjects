using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoInfrastructure.Models
{
    public class Group
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AdminId { get; set; } = string.Empty;

        public ICollection<User> Members { get; set; }
    }
}
