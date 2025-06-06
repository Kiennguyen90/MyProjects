using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoInfrastructure.Model
{
    public class Service
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<UserService> UserServices { get; set; }

        public ICollection<ServiceType> ServiceTypes { get; set; }

        public Service()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
