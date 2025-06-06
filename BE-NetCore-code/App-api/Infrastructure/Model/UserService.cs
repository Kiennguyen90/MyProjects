using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Model
{
    public class UserService
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ServiceId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        public UserRoleService UserRoleService { get; set; }

        public UserService() {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.UtcNow;
            ExpireDate = DateTime.UtcNow.AddYears(1);
            IsActive = true;
        }
    }
}
