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


        public async Task<List<Product>> GetProductsAsync(int idStore)
        {
            var consulta = from datos in this.context.Products
                           where datos.StoreId == idStore
                           select datos;

            return await consulta.ToListAsync();
        }


        //public async Task InsertarCuboAsync(string nombre, string modelo, string marca, string imagen, int precio)
        //{
        //    int maxId = await this.context.Cubos.MaxAsync(x => x.IdCubo);

        //    Cubo c = new Cubo
        //    {
        //        IdCubo = maxId + 1,
        //        Nombre = nombre,
        //        Modelo = modelo,
        //        Marca = marca,
        //        Imagen = imagen,
        //        Precio = precio
        //    };

        //    await this.context.Cubos.AddAsync(c);
        //    await this.context.SaveChangesAsync();
        //}

    }
}
