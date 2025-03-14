using Microsoft.AspNetCore.Mvc;
using Eshop.Models;
using Eshop.Repositories;
using Eshop.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Eshop.Controllers
{
    public class UsersController : Controller
    {
        private RepositoryStores repoStores;
        private RepositoryUsers repoUsers;
        private RepositoryPayment repoPay;
        private const string UserKey = "UserId";

        public UsersController(RepositoryStores repoStores, RepositoryUsers repoUsers, RepositoryPayment repoPay) {
            this.repoStores = repoStores;
            this.repoUsers = repoUsers;
            this.repoPay = repoPay;
        }


        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password) {
            User user = await this.repoUsers.LoginAsync(email, password);
            if (user == null) {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            HttpContext.Session.SetObject(UserKey, user.Id);
            return RedirectToAction("Profile", "Users");
        }


        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string confirmpassword, string telephone, string address) {
            User user = await this.repoUsers.InsertUserAsync(name, email, password, telephone, address);
            if (user == null) {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            HttpContext.Session.SetObject(UserKey, user.Id);
            return RedirectToAction("Profile", "Users");
        }

        public IActionResult Logout() {
            HttpContext.Session.Remove(UserKey);
            return RedirectToAction("Home", "Home");
        }


        public async Task<IActionResult> Profile() {
            int userId = HttpContext.Session.GetObject<int>(UserKey);

            if (userId == 0) {
                return RedirectToAction("Home", "Home");
            }

            User user = await this.repoUsers.FindUserAsync(userId);
            if (user == null) {
                return RedirectToAction("Home", "Home");
            }


            Store store = await this.repoUsers.FindStoreByUserIdAsync(userId);

            ViewBag.Store = store;

            List<Purchase> purchases = await this.repoPay.GetPurchasesByUserIdAsync(userId);

            if (purchases != null) {
                ViewBag.Purchases = purchases;
            }

            return View(user);
        }

    }
}
