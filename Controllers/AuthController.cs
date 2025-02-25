using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
