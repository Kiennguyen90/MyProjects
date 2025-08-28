namespace CryptoInfrastructure
{
    using CryptoInfrastructure.Models;
    using Microsoft.EntityFrameworkCore;

    public class CryptoDbcontext : DbContext
    {
        public CryptoDbcontext(DbContextOptions<CryptoDbcontext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entities here
            modelBuilder.Entity<CryptoToken>()
                .HasIndex(ct => ct.Symbol)
                .IsUnique();
            modelBuilder.Entity<UserToken>()
                .HasIndex(ut => new { ut.UserId, ut.TokenId })
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
        // DbSets for your entities
        public DbSet<CryptoToken> CryptoTokens { get; set; }
        public DbSet<UserCryptoExchange> UserCryptoExchanges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserBalanceAction> UserBalanceActions { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
    }
}
