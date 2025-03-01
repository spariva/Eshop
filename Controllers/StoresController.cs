using Eshop.Data;
using Eshop.Helpers;
using Eshop.Models;
using Eshop.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class StoresController : Controller
    {
        private RepositoryStores repoStores;
        private HelperPathProvider helperPath;

        public StoresController(RepositoryStores repoStores, HelperPathProvider helperPath) 
        {
            this.repoStores = repoStores;
            this.helperPath = helperPath;
        }

        #region Stores CRUD
        public async Task<IActionResult> Stores()
        {
            List<Store> stores = await this.repoStores.GetStoresAsync();
            ViewBag.Categories = stores.Select(x => x.Category).Distinct().ToList();
            return View(stores);
        }

        public async Task<IActionResult> StoreDetails(int id)
        {
            //Find store and add their products loading the ProdCats and Categories
            StoreView storeView = await this.repoStores.FindStoreAsync(id);
            if (storeView == null)
            {
                return RedirectToAction("Stores");
            }

            return View(storeView);
        }

        public IActionResult StoreCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StoreCreate(string name, string email, IFormFile image, string category)
        {
            //Create route and save image
            string fileName = image.FileName;
            string path = this.helperPath.MapPath(fileName, Folder.Stores);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            //Insert store
            Store store = await this.repoStores.InsertStoreAsync(name, email, fileName, category.ToUpper());

            return RedirectToAction("StoreDetails", new {id = store.Id} );
        }


        #endregion

        #region Producs CRUD
        public async Task<IActionResult> ProductList()
        {
            List<Store> stores = await this.repoStores.GetStoresAsync();
            return View(stores);
        }

        public async Task<IActionResult> ProductDetails()
        {
            List<Store> stores = await this.repoStores.GetStoresAsync();
            return View(stores);
        }



        #endregion
    }
}
