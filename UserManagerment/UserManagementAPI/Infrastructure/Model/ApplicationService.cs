using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class ApplicationService
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ServiceId { get; set; }

        public ApplicationUser Users { get; set; }
        public ApplicationUserService Services { get; set; }
    }
}
