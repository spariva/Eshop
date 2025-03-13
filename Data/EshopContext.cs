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

            // Configure payment-related relationships

            // Purchase to User relationship
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            // PurchaseItem to Purchase relationship
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.PurchaseId);

            // PurchaseItem to Product relationship
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductId);

            // PurchaseVendorMapping to Purchase relationship
            modelBuilder.Entity<PurchaseVendorMapping>()
                .HasOne(pvm => pvm.Purchase)
                .WithMany(p => p.PurchaseVendorMappings)
                .HasForeignKey(pvm => pvm.PurchaseId);

            // PurchaseVendorMapping to Store relationship
            modelBuilder.Entity<PurchaseVendorMapping>()
                .HasOne(pvm => pvm.Store)
                .WithMany()
                .HasForeignKey(pvm => pvm.VendorId);

            // Payment to Purchase relationship
            modelBuilder.Entity<Payment>()
                .HasOne(pay => pay.Purchase)
                .WithMany()
                .HasForeignKey(pay => pay.PurchaseId);

            // Payment to User relationship
            modelBuilder.Entity<Payment>()
                .HasOne(pay => pay.User)
                .WithMany()
                .HasForeignKey(pay => pay.UserId);

            // StorePayout to Store relationship
            modelBuilder.Entity<StorePayout>()
                .HasOne(sp => sp.Store)
                .WithMany()
                .HasForeignKey(sp => sp.StoreId);

            // StorePayout to Purchase relationship
            modelBuilder.Entity<StorePayout>()
                .HasOne(sp => sp.Purchase)
                .WithMany()
                .HasForeignKey(sp => sp.PurchaseId);
        }

    }
}
