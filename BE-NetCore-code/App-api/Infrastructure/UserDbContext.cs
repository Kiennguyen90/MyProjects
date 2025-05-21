namespace Infrastructure
{
    using Infrastructure.Model;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection.Emit;

    public class UserDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) :
        base(options)
        { }
        public DbSet<ApplicationUserToken> ApplicationUserTokens { get; set; }
        public DbSet<UserService> UserServices { get; set; }
        public DbSet<Service> Services { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<ApplicationUserService>()
            //.HasOne(c => c.Users)
            //.WithMany(p => p.ApplicationUserServices)
            //.HasForeignKey(c => c.UserId);

            //builder.Entity<ApplicationUserService>()
            //.HasOne(c => c.Services)
            //.WithMany(p => p.ApplicationUserServices)
            //.HasForeignKey(c => c.ServiceId);
            base.OnModelCreating(builder);
        }
    }
}
