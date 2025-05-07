using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class CryptoInvestmentContext : DbContext
    {
        public CryptoInvestmentContext(DbContextOptions<CryptoInvestmentContext> options)
            : base(options)
        {
        }
    }
}
