using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Model
{
    public class UserRoleService
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public string UserServiceId { get; set; }

        [Required]
        public string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public IdentityRole Role { get; set; }
    }
}
