using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Model
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpireTime { get; set; }
        public string DeviceName { get; set; }
    }
}
