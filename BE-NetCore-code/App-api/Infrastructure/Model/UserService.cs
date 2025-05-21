using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class UserService
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ServiceId { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
    }
}
