using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
