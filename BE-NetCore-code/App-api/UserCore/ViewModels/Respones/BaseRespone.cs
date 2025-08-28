using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCore.ViewModels.Respones
{
    public class BaseRespone
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
