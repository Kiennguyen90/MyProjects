using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCore.ViewModels.Requests
{
    public class RegisterServiceRequest
    {
        public string ServiceId { get; set; } = string.Empty;

        public int TypeId { get; set; }
    }
}
