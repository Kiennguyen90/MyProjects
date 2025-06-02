using Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCore.ViewModels.Respones
{
    public class UserComonInfoRespone
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public string userRole { get; set; }
        public List<UserServiceRespone> Services { get; set; }
    }

    public class UserServiceRespone
    {
        public string ServiceId { get; set; }
        public string RoleId { get; set; }
    }
}
