
using Microsoft.EntityFrameworkCore;
using SmartShopAI.Models;

namespace SmartShopAI.Data
{
    public class SmartShopDbContext : DbContext
    {
        public SmartShopDbContext(
            DbContextOptions<SmartShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
            = null!;

        public DbSet<Review> Reviews { get; set; }
            = null!;

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .ToTable("Product");

            modelBuilder.Entity<Review>()
                .ToTable("Review");

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId);
        }
    }
}
