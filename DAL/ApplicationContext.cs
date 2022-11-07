using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Goods> Goods { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        private string ConnectionString;

        public ApplicationContext(string connectionString= "Data Source=Goods.db")
        {
            ConnectionString = connectionString;
            if(!ConnectionString.Equals("Data Source=Goods.db"))
            {
                Database.EnsureCreated();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}
