using System.IO;
using Microsoft.EntityFrameworkCore;
using Shelfy.Models;

namespace Shelfy.Data
{
    public class ShelfyDbContext : DbContext
    {
        public ShelfyDbContext(DbContextOptions<ShelfyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public static string GetDatabasePath()
        {
            DirectoryInfo? directory = new(AppContext.BaseDirectory);

            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Shelfy.slnx")))
                {
                    return Path.Combine(directory.FullName, "shelfy.db");
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException(
                "Could not locate the Shelfy solution folder for the database.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={GetDatabasePath()}");
            }
        }
    }
}