using Microsoft.AspNetCore.Mvc;
using Eshop.Models;
using Eshop.Repositories;
using Eshop.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Eshop.Controllers
{
    public class UsersController : Controller
    {
        private RepositoryStores repoStores;
        private RepositoryUsers repoUsers;
        private const string UserKey = "UserId"; //if I change this, so I have to change it in the UserMenu partialview

        public UsersController(RepositoryStores repoStores, RepositoryUsers repoUsers) {
            this.repoStores = repoStores;
            this.repoUsers = repoUsers;
        }

        public IActionResult Login() {
            HttpContext.Session.SetObject(UserKey, 3);
            return RedirectToAction("Profile");

        }

        public IActionResult Register() {
            return RedirectToAction("Home", "Home");
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


            return View(user);
        }

    }
}
