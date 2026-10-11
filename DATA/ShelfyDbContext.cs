using Microsoft.EntityFrameworkCore;
using Shelfy.Models;

namespace Shelfy.Data
{
    public class ShelfyDbContext : DbContext
    {
        public ShelfyDbContext(DbContextOptions<ShelfyDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=shelfy.db");
            }
        }
    }
}