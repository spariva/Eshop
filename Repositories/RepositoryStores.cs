using Eshop.Data;
using Eshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Repositories
{
    public class RepositoryStores
    {
        private EshopContext context;

        public RepositoryStores(EshopContext context)
        {
            this.context = context;
        }

#region Stores
        public async Task<List<Store>> GetStoresAsync()
        {
            var consulta = from datos in this.context.Stores
                           select datos;
            return await consulta.ToListAsync();
        }


        public async Task<StoreView> FindStoreAsync(int idStore)
        {
            var consulta = from datos in this.context.Stores
                           where datos.Id == idStore
                           select datos;

            Store store = await consulta.FirstOrDefaultAsync();


            if (store == null)
            {
                return null;
            }

            // Get the products for the store, including their ProdCats and Categories
            List<Product> products = await this.context.Products
                .Where(p => p.StoreId == idStore) // Filter by store ID
                .Include(p => p.ProdCats) // Include ProdCats
                .ThenInclude(pc => pc.Category) // Include Category for each ProdCat
                .ToListAsync();

            // Get distinct category names for the filter list
            var categoryNames = products
                .SelectMany(p => p.ProdCats.Select(pc => pc.Category.CategoryName))
                .Distinct()
                .ToList();

            // Create the StoreView
            StoreView storeView = new StoreView()
            {
                Store = store,
                Products = products,
                ProdCategories = categoryNames
            };

            return storeView;
        }

        public async Task<Store> InsertStoreAsync(string name, string email, string image, string category)
        {
            int maxId = await this.context.Stores.MaxAsync(x => x.Id);

            Store s = new Store
            {
                Id = maxId + 1,
                Name = name,
                Email = email,
                Image = image,
                Category = category
            };

            await this.context.Stores.AddAsync(s);
            await this.context.SaveChangesAsync();
            return s;
        }

        #endregion


        public async Task<List<Product>> GetProductsAsync(int idStore)
        {
            var consulta = from datos in this.context.Products
                           where datos.StoreId == idStore
                           select datos;

            return await consulta.ToListAsync();
        }

        public async Task<List<string>> GetProductsCategoriesAsync(List<int> productsIds)
        {
            var consulta = this.context.ProdCats
                .Where(pc => productsIds.Contains(pc.ProductId))
                .Join(this.context.Categories,
                    pc => pc.CategoryId,
                    c => c.Id,
                    (pc, c) => c.CategoryName)
                .Distinct();

            return await consulta.ToListAsync();
        }

    }
}
