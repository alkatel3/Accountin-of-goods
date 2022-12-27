using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DAL.EF
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<Goods> Goods { get; set; }
        public virtual DbSet<OrderList> OrderLists { get; set; }
        public virtual DbSet<QueueForPurchase> QueueForPurchases { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<GoodsInStock> GoodsInStock { get; set; }

        private string ConnectionString;
        public ApplicationContext(string connectionString = 
            @"Server=(localdb)\mssqllocaldb;Database=Goods;Trusted_Connection=True;") 
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(c => c.OrderList)
                .WithOne(ol => ol.User)
                .HasForeignKey<OrderList>(ol => ol.UserId)
                .HasConstraintName("ForeignKey_OrderList_User")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderList>()
                .HasOne(ol => ol.User)
                .WithOne(c => c.OrderList)
                .HasForeignKey<OrderList>(ol => ol.UserId)
                .HasConstraintName("ForeignKey_OrderList_User")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderList)
                .WithMany(ol => ol.Orders)
                .HasForeignKey(o => o.OrderListId)
                .HasConstraintName("ForeignKey_Order_OrderList")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goods>()
                .HasMany(g => g.Orders)
                .WithOne(o => o.Goods)
                .HasForeignKey(o => o.GoodsId)
                .HasConstraintName("ForeignKey_Order_Goods");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Goods)
                .WithMany(g => g.Orders)
                .HasForeignKey(o => o.GoodsId)
                .HasConstraintName("ForeignKey_Order_Goods")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QueueForPurchase>()
                .HasOne(qp => qp.Goods)
                .WithOne(o => o.QueueForPurchase)
                .HasForeignKey<QueueForPurchase>(qp => qp.GoodsId)
                .HasConstraintName("ForeignKey_QueueForPurchase_Goods")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GoodsInStock>()
                .HasOne(gs => gs.Goods)
                .WithOne(g => g.GoodsInStock)
                .HasForeignKey<GoodsInStock>(qs => qs.GoodsId)
                .HasConstraintName("ForeignKey_GoodsInStock_Goods")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasAlternateKey(c => c.PhoneNumber);

            modelBuilder.Entity<User>()
                .HasKey(c => c.Id)
                .HasName("PrimaryKey_UserId");

            modelBuilder.Entity<QueueForPurchase>()
                .HasKey(c => c.Id)
                .HasName("PrimaryKey_QueueForPurchaseId");

            modelBuilder.Entity<OrderList>()
                .HasKey(c => c.Id)
                .HasName("PrimaryKey_OrderListId");

            modelBuilder.Entity<Order>()
                .HasKey(c => c.Id)
                .HasName("PrimaryKey_OrderId");

            modelBuilder.Entity<GoodsInStock>()
                .HasKey(c => c.Id)
                .HasName("PrimaryKey_GoodsInStockId");
            
            modelBuilder.Entity<Goods>()
                .HasKey(c => c.Id)
                .HasName("PrimaryKey_Goods");

            modelBuilder.Entity<Goods>()
                .Property(p => p.Version)
                .IsRowVersion();

            modelBuilder.Entity<GoodsInStock>()
                .Property(p => p.Version)
                .IsRowVersion();

            modelBuilder.Entity<Order>()
                .Property(p => p.Version)
                .IsRowVersion();

            modelBuilder.Entity<QueueForPurchase>()
                .Property(p => p.Version)
                .IsRowVersion();

            modelBuilder.Entity<OrderList>()
                .Property(p => p.Version)
                .IsRowVersion();

            modelBuilder.Entity<User>()
                .Property(p => p.Version)
                .IsRowVersion();
        }
    }
}
