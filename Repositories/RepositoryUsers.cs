using Eshop.Data;
using Eshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Repositories
{
    public class RepositoryUsers
    {
        private EshopContext context;

        public RepositoryUsers(EshopContext context)
        {
            this.context = context;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var consulta = from datos in this.context.Users
                           select datos;

            List<User> users = await consulta.ToListAsync();
            return users;
        }

        public async Task<User> FindUserAsync(int id)
        {
            var consulta = from datos in this.context.Users
                           where datos.Id == 1
                           select datos;

            User user = await consulta.FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<Store> FindStoreByUserIdAsync(int userId)
        {
            var consulta = from datos in this.context.Stores
                           where datos.UserId == userId
                           select datos;

            Store store = await consulta.FirstOrDefaultAsync();

            if (store == null) 
            { 
                return null;
            }

            return store;
        }
    }
}
