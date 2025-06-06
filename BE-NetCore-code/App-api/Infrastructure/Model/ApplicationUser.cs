using Microsoft.AspNetCore.Identity;

namespace CryptoInfrastructure.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool Gender { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<UserService> UserServices { get; set; }
    }
}
