using Eshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Data
{
    public class EshopContext: DbContext
    {
        public EshopContext(DbContextOptions<EshopContext> options): base(options) {  }

        public DbSet<User> Users { get; set; }
    }
}
