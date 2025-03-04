﻿using Eshop.Data;
using Eshop.Helpers;
using Eshop.Models;
using Eshop.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> StoreEdit(int id)
        {
            Store store = await this.repoStores.FindSimpleStoreAsync(id);
            return View(store);
        }

        [HttpPost]
        public async Task<IActionResult> StoreEdit(int id, string name, string email, IFormFile image, string oldimage, string category)
        {
            try
            {


                //Update image
                if (image != null)
                {
                    string fileName = image.FileName;
                    string path = this.helperPath.MapPath(fileName, Folder.Stores);
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    await this.repoStores.UpdateStoreAsync(id, name, email, fileName, category);

                }
                else
                {
                    await this.repoStores.UpdateStoreAsync(id, name, email, oldimage, category);
                }


                return RedirectToAction("StoreDetails", new { id = id });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use any logging framework)
                Console.WriteLine($"Error updating store: {ex.Message}");
                // Optionally, add a user-friendly error message to the view
                ModelState.AddModelError(string.Empty, "An error occurred while updating the store. Please try again.");
                // Return the view with the current model to display the error
                Store store = await this.repoStores.FindSimpleStoreAsync(id);
                return View(store);
            }
        }

        public async Task<IActionResult> StoreDelete(int id)
        {
            await this.repoStores.DeleteStoreAsync(id);
            return RedirectToAction("Stores");
        }


        #endregion

        #region Producs CRUD
        public async Task<IActionResult> ProductList()
        {
            List<Product> products = await this.repoStores.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            Product product = await this.repoStores.FindProductAsync(id);
            return View(product);
        }

        public async Task<IActionResult> ProductCreate()
        {
            List<Category> categories= await this.repoStores.GetAllCategoriesAsync();
            ViewBag.Productcategories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(string name, string description, IFormFile image, decimal price, int stockQuantity, List<int> selectedCategories, string newCategories)
        {
            if (ModelState.IsValid)
            {
                // Save the image
                string fileName = image.FileName;
                string path = this.helperPath.MapPath(fileName, Folder.Products);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Handle new categories
                if (!string.IsNullOrEmpty(newCategories))
                {
                    var newCategoryNames = newCategories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
                    foreach (var categoryName in newCategoryNames)
                    {
                        var category = await this.repoStores.FindOrCreateCategoryAsync(categoryName);
                        selectedCategories.Add(category.Id);
                    }
                }

                // Insert the product
                var product = await this.repoStores.InsertProductAsync(name, description, fileName, price, stockQuantity, selectedCategories);

                return RedirectToAction("ProductDetails", new { id = product.Id });
            }

            // If we got this far, something failed; re-populate the categories
            var categories = await this.repoStores.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View();
        }





        #endregion
    }
}
