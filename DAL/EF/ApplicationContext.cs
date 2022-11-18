using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<Goods> Goods { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;

        private string ConnectionString;

        public ApplicationContext(string connectionString = "Data Source=Goods.db"):base()
        {
            ConnectionString = "Data Source=Goods.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}
