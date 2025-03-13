using Microsoft.AspNetCore.Mvc;
using Eshop.Models;
using Eshop.Repositories;
using Eshop.Extensions;

namespace Eshop.Controllers
{
    public class CartController : Controller
    {
        private RepositoryStores repo;
        private const string CartKey = "CartItems";
        private const decimal Shipping = 10;
        private const decimal Offer = 75;

        public CartController(RepositoryStores repoStores)
        {
            repo = repoStores;
        }

        private async Task<decimal> CalculateCartSubtotal(List<CartItem> cartItems)
        {
            decimal cartSubtotal = 0;
            foreach (var cartItem in cartItems)
            {
                Product product = await this.repo.FindProductAsync(cartItem.Id);
                if (product != null)
                {
                    cartSubtotal += product.Price * cartItem.Quantity;
                }
            }
            return cartSubtotal;
        }

        public async Task<IActionResult> Cart()
        {
            List<CartItem> cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartKey);

            if (cartItems == null)
            {
                ViewBag.Mensaje = "Your shopping cart is empty!";
                return View();
            }
            else if (cartItems.Count == 0)
            {
                ViewBag.Mensaje = "Your shopping cart is empty!";
                HttpContext.Session.Remove(CartKey);
                return View();
            }


            List<Product> products = await this.repo.GetCartItemsAsync(cartItems);


            return View(products);
        }

        public IActionResult AddToCart(int id)
        {
            List<CartItem> cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartKey);

            if (cartItems == null)
            {
                cartItems = new List<CartItem>() { new CartItem { Id = id, Quantity = 1 } };
                HttpContext.Session.SetObject(CartKey, cartItems);
                return Json(new { success = true });
            }


            var item = cartItems.FirstOrDefault(c => c.Id == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem { Id = id, Quantity = 1 });
            }

            HttpContext.Session.SetObject(CartKey, cartItems);
            ViewBag.Mensaje = "Item added to cart!";
            return Json(new { success = true });
        }


        [HttpGet]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            List<CartItem> cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartKey);

            cartItems.RemoveAll(c => c.Id == id);
            HttpContext.Session.SetObject(CartKey, cartItems);


            // Calculate cart totals
            decimal cartSubtotal = await this.CalculateCartSubtotal(cartItems);
            decimal shipping = cartItems.Count > 0 && cartSubtotal < Offer ? Shipping : 0;
            decimal cartTotal = cartSubtotal + shipping;

            return Json(new
            {
                success = true,
                shipping = shipping,
                cartSubtotal = cartSubtotal,
                cartTotal = cartTotal,
                itemCount = cartItems.Count
            });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            List<CartItem> cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartKey);
            var item = cartItems.FirstOrDefault(c => c.Id == id);

            if (item == null)
            {
                return Json(new { success = false });
            }

            // Ensure quantity is at least 1
            quantity = Math.Max(1, quantity);
            item.Quantity = quantity;

            HttpContext.Session.SetObject(CartKey, cartItems);

            Product product = await this.repo.FindProductAsync(id);
            if (product == null)
            {
                return Json(new { success = false });
            }

            // Calculate cart totals
            decimal cartSubtotal = await this.CalculateCartSubtotal(cartItems);
            // Shipping is the const but if the cart is empty the shipping is 0
            decimal shipping = cartItems.Count > 0 && cartSubtotal < Offer ? Shipping : 0;
            decimal cartTotal = cartSubtotal + shipping;
            decimal subtotal = product.Price * quantity;


            return Json(new
            {
                success = true,
                shipping = shipping,
                subtotal = subtotal,
                cartSubtotal = cartSubtotal,
                cartTotal = cartTotal
            });
        }

    }
}
