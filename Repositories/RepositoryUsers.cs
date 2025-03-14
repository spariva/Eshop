using Eshop.Data;
using Eshop.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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
                           where datos.Id == id
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

        public async Task<User> LoginAsync(string email, string password) {
            var consulta = from datos in this.context.Users
                           where datos.Email == email
                           select datos;

            User user = await consulta.FirstOrDefaultAsync();

            if (user == null) {
                return null;
            }

            //Estoy comparando el password con el salt, hasta que meta criptografia este finde
            if (user.Salt != password) {
                return null;
            }
            //if (!PasswordHelper.VerifyPassword(password, user.PasswordHash, user.Salt)) {
            //    return null;
            //}
            return user;
        }

        public async Task<User> InsertUserAsync(string name, string email, string password, string telephone, string address) {
            var consulta = from datos in this.context.Users
                           where datos.Email == email
                           select datos;

            if (await consulta.FirstOrDefaultAsync() != null) {
                return null;
            }

            int maxId = this.context.Users.Max(u => u.Id);
            
            
            User user = new User()
            {
                Id = maxId + 1,
                Name = name,
                Email = email,
                PasswordHash = new byte[] { 0x12, 0x34 }, // Example byte array
                Salt = password,
                Telephone = telephone,
                Address = address
            };

            this.context.Users.Add(user);
            try {
                await this.context.SaveChangesAsync();
                return user;
            }
            catch (Exception e) {
                return null;
            }
        }


    }
}
