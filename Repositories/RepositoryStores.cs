using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<Store> FindSimpleStoreAsync(int idStore)
        {
            var consulta = from datos in this.context.Stores
                           where datos.Id == idStore
                           select datos;

            Store store = await consulta.FirstOrDefaultAsync();

            return store;
        }

        public async Task<StoreView> FindStoreAsync(int idStore)
        {
            var consulta = from datos in this.context.Stores
                           where datos.Id == idStore
                           select datos;

            Store store = await consulta.FirstOrDefaultAsync();

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


        public async Task<Store> CreateStoreAsync(string name, string email, string image, string category, int userId, string stripeId)
        {
            int maxId = await this.context.Stores.MaxAsync(x => x.Id);

            Store s = new Store
            {
                Id = maxId + 1,
                Name = name,
                Email = email,
                Image = image,
                Category = category,
                UserId = userId,
                StripeId = stripeId
            };

            await this.context.Stores.AddAsync(s);
            await this.context.SaveChangesAsync();
            return s;
        }

        public async Task<Store> UpdateStoreAsync(int id, string name, string email, string image, string category)
        {
            Store s = await this.FindSimpleStoreAsync(id);

            s.Name = name;
            s.Email = email;
            s.Image = image;
            s.Category = category;
            await this.context.SaveChangesAsync();
            return s;
        }

        public async Task DeleteStoreAsync(int id)
        {
            Store s = await this.FindSimpleStoreAsync(id);
            this.context.Stores.Remove(s);
            await this.context.SaveChangesAsync();
        }

        #endregion


        #region product categories
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var consulta = from datos in this.context.Categories
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<Category> FindOrCreateCategoryAsync(string categoryName)
        {
            var category = await this.context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName.ToUpper());
            int maxId = await this.context.Categories.MaxAsync(x => x.Id);
            if (category == null)
            {
                category = new Category { Id = (maxId + 1), CategoryName = categoryName };
                this.context.Categories.Add(category);
                await this.context.SaveChangesAsync();
            }
            return category;
        }

        public async Task AddCategoryToProductAsync(int productId, int categoryId)
        {
            var prodCat = new ProdCat { ProductId = productId, CategoryId = categoryId };
            this.context.ProdCats.Add(prodCat);
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveCategoryToProductAsync(int productId, int categoryId)
        {
            var prodCat = await this.context.ProdCats.FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId);
            if (prodCat != null)
            {
                this.context.ProdCats.Remove(prodCat);
                await this.context.SaveChangesAsync();
            }
        }
        #endregion


        #region products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var consulta = await this.context.Products.ToListAsync();
            // Get the products for the store, including their ProdCats and Categories
            //List<Product> products = await this.context.Products
            //    .Include(p => p.ProdCats) // Include ProdCats
            //    .ThenInclude(pc => pc.Category) // Include Category for each ProdCat
            //    .ToListAsync();

            //// Get distinct category names for the filter list
            //var categoryNames = products
            //    .SelectMany(p => p.ProdCats.Select(pc => pc.Category.CategoryName))
            //    .Distinct()
            //    .ToList();

            return consulta;
        }

        public async Task<Product> FindProductAsync(int idProduct)
        {
            //var consulta = from datos in this.context.Products
            //               where datos.Id == idProduct
            //               select datos;

            var product = this.context.Products
                .Where(p => p.Id == idProduct) // Filter by product ID
                .Include(p => p.ProdCats) // Include ProdCats
                .ThenInclude(pc => pc.Category); // Include Category for each ProdCat

            if (product == null)
            {
                return null;
            }
            Product p = await product.FirstOrDefaultAsync();

            return p;
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


        public async Task<Product> SearchProductByNameAsync(string name)
        {
            var consulta = from datos in this.context.Products
                           where datos.Name == name
                           select datos;

            return await consulta.FirstOrDefaultAsync();
        }

        #region CRUD Products
        public async Task<Product> CreateProductAsync(string name, int storeId, string description, string image, decimal price, int stock, List<int> categories)
        {
            int maxId = await this.context.Products.MaxAsync(x => x.Id);
            Product p = new Product
            {
                Id = maxId + 1,
                StoreId = storeId,
                Name = name,
                Description = description,
                Image = image,
                Price = price,
                StockQuantity = stock
            };
            await this.context.Products.AddAsync(p);
            await this.context.SaveChangesAsync();

            foreach (int cat in categories)
            {
                ProdCat pc = new ProdCat
                {
                    ProductId = p.Id,
                    CategoryId = cat
                };
                await this.context.ProdCats.AddAsync(pc);
            }
            await this.context.SaveChangesAsync();
            return p;
        }

        public async Task<Product> UpdateProductAsync(int id, string name, string description, string image, decimal price, int stock, List<int> categories)
        {
            Product p = await this.FindProductAsync(id);

            p.Name = name;
            p.Description = description;
            p.Image = image;
            p.Price = price;
            p.StockQuantity = stock;

            foreach (int cat in categories) {
                ProdCat existingProdCat = await this.context.ProdCats
                    .FirstOrDefaultAsync(pc => pc.ProductId == p.Id && pc.CategoryId == cat);

                if (existingProdCat == null) {
                    ProdCat pc = new ProdCat
                    {
                        ProductId = p.Id,
                        CategoryId = cat
                    };
                    await this.context.ProdCats.AddAsync(pc);
                }
            }

            await this.context.SaveChangesAsync();
            return p;
        }

        public async Task DeleteProductAsync(Product p)
        {
            this.context.Products.Remove(p);
            await this.context.SaveChangesAsync();
        }



        #endregion

        #endregion

        #region Shopping-Cart

        public async Task<List<Product>> GetCartItemsAsync(List<CartItem> cartItems)
        {
            var consulta = from datos in this.context.Products
                           where cartItems.Select(ci => ci.Id).Contains(datos.Id)
                           select datos;

            if (consulta == null)
            {
                return null;
            }

            return await consulta.ToListAsync();
        }

        //public async Task<CartItem> FindCartItemAsync(int userId, int productId)
        //{
        //    var consulta = from datos in this.context.CartItems
        //                   where datos.UserId == userId && datos.ProductId == productId
        //                   select datos;
        //    return await consulta.FirstOrDefaultAsync();
        //}

        //public async Task<CartItem> AddCartItemAsync(int userId, int productId, int quantity)
        //{
        //    CartItem ci = new CartItem
        //    {
        //        UserId = userId,
        //        ProductId = productId,
        //        Quantity = quantity
        //    };
        //    await this.context.CartItems.AddAsync(ci);
        //    await this.context.SaveChangesAsync();
        //    return ci;
        //}

        //public async Task<CartItem> UpdateCartItemAsync(int userId, int productId, int quantity)
        //{
        //    CartItem ci = await this.FindCartItemAsync(userId, productId);
        //    ci.Quantity = quantity;
        //    await this.context.SaveChangesAsync();
        //    return ci;
        //}

        //public async Task DeleteCartItemAsync(int userId, int productId)
        //{
        //    CartItem ci = await this.FindCartItemAsync(userId, productId);
        //    this.context.CartItems.Remove(ci);
        //    await this.context.SaveChangesAsync();
        //}

        #endregion
    }
}
