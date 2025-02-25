using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
