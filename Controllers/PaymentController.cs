using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
