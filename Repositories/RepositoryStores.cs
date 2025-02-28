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

            List<Product> products = await this.GetProductsAsync(idStore);

            StoreView storeView = new StoreView()
            {
                Store = store,
                Products = products
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

    }
}
