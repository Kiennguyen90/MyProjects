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
        }
        // DbSets for your entities
        public DbSet<CryptoToken> CryptoTokens { get; set; }
        public DbSet<UserCryptoExchange> UserCryptoExchanges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}
