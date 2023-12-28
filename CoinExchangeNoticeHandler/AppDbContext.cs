using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoinExchangeNoticeHandler
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions options) : base(options) { }
        public static string ConnectionString => "Filename=db";
        public DbSet<Bithumb> Bithumb { get; set; } = null!;
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(AppDbContext.ConnectionString);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
