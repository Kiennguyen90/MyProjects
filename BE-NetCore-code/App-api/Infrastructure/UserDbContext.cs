﻿namespace Infrastructure
{
    using Infrastructure.Model;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection.Emit;

    public class UserDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<ApplicationUserToken> ApplicationUserTokens { get; set; }
        public DbSet<UserService> UserServices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<UserRoleService> UserRoleServices { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserService>()
            .HasOne(c => c.User)
            .WithMany(p => p.UserServices)
            .HasForeignKey(c => c.UserId);

            builder.Entity<UserService>()
            .HasOne(c => c.Service)
            .WithMany(p => p.UserServices)
            .HasForeignKey(c => c.ServiceId);
            base.OnModelCreating(builder);
        }
    }
}
