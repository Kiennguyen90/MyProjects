using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class Service
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }


        public ICollection<UserService> UserServices { get; set; }
    }
}
