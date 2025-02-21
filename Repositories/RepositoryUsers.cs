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
    }
}
