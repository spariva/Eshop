using Eshop.Models;
using Eshop.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class StoresController : Controller
    {
        private RepositoryStores repoStores;

        public StoresController(RepositoryStores repoStores) 
        {
            this.repoStores = repoStores;
        }

        public async Task<IActionResult> Index()
        {
            List<Store> stores = await this.repoStores.GetStoresAsync();
            return View(stores);
        }

        //public async Task<IActionResult> Details(int id)
        //{
        //    Store store = await this.repoStores.FindStoreAsync(id);
        //    return View(store);
        //}

        public async Task<IActionResult> ProductList()
        {
            List<Store> stores = await this.repoStores.GetStoresAsync();
            return View(stores);
        }
    }
}
