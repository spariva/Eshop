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

    }
}
