using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class ApplicationUserService
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ServiceId { get; set; }
        public ApplicationUser User{ get; set; }
        public ApplicationService Service { get; set; }
    }
}
