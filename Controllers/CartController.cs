﻿using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }
    }
}
