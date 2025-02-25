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

        public async Task<Store> FindStoreAsync(int idStore)
        {
            var consulta = from datos in this.context.Stores
                           where datos.Id == idStore
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }


    }
}
