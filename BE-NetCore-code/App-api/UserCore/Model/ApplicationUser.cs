using Microsoft.AspNetCore.Identity;

namespace UserCore.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool Gender { get; set; }
    }
}
