using Eshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Data
{
    public class EshopContext: DbContext
    {
        public EshopContext(DbContextOptions<EshopContext> options): base(options) {  }

        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProdCat> ProdCats { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<PurchaseVendorMapping> PurchaseVendorMappings { get; set; }
        public DbSet<StorePayout> StorePayouts { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the many-to-many relationship
            modelBuilder.Entity<ProdCat>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId }); // Composite primary key

            // Configure the relationship between ProdCat and Product
            modelBuilder.Entity<ProdCat>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProdCats)
                .HasForeignKey(pc => pc.ProductId);

            // Configure the relationship between ProdCat and Category
            modelBuilder.Entity<ProdCat>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProdCats)
                .HasForeignKey(pc => pc.CategoryId);
        }

    }
}
