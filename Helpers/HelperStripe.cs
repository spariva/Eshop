//using Eshop.Models;
//using Eshop.Repositories;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using static Eshop.Repositories.PaymentController;

//namespace Eshop.Controllers
//{
//    [Authorize]
//    public class PaymentController : Controller
//    {
//        private readonly IPaymentRepository paymentRepository;

//        public PaymentController(IPaymentRepository paymentRepository) {
//            this.paymentRepository = paymentRepository;
//        }

//        // GET: Payment/Checkout
//        [HttpGet]
//        public async Task<IActionResult> Checkout(List<CartItem> cartItems) {
//            try {
//                // Get current user ID
//                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

//                // Validate cart items and check stock
//                var (isValid, invalidItems) = await paymentRepository.ValidateCartItemsAsync(cartItems);

//                if (!isValid) {
//                    // Return information about invalid items
//                    return BadRequest(new { Message = "Some items in your cart are not available", InvalidItems = invalidItems });
//                }

//                // Get products for display
//                var products = await paymentRepository.GetCartProductsAsync(cartItems);

//                // Group items by store for display
//                var itemsByStore = await paymentRepository.GroupCartItemsByStoreAsync(cartItems);

//                // Store cart items in TempData (serialize to JSON or use session)
//                TempData["CartItems"] = System.Text.Json.JsonSerializer.Serialize(cartItems);

//                // Return view with checkout information
//                return View(new CheckoutViewModel
//                {
//                    CartItems = cartItems,
//                    Products = products,
//                    ItemsByStore = itemsByStore
//                });
//            }
//            catch (Exception ex) {
//                return StatusCode(500, new { Message = "An error occurred during checkout", Error = ex.Message });
//            }
//        }

//        // POST: Payment/InitiatePayment
//        [HttpPost]
//        public async Task<IActionResult> InitiatePayment() {
//            try {
//                // Get current user ID
//                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

//                // Get cart items from TempData
//                var cartItemsJson = TempData["CartItems"] as string;
//                var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson);

//                // Revalidate cart items and check stock
//                var (isValid, invalidItems) = await paymentRepository.ValidateCartItemsAsync(cartItems);

//                if (!isValid) {
//                    return BadRequest(new { Message = "Some items in your cart are not available", InvalidItems = invalidItems });
//                }

//                // Create a Stripe checkout session (simplified - actual Stripe integration would be needed)
//                string stripeSessionId = "sess_" + Guid.NewGuid().ToString("N");

//                // Create purchase record in database
//                var purchase = await paymentRepository.CreatePurchaseAsync(userId, cartItems, stripeSessionId);

//                // Redirect to Stripe checkout page (simplified)
//                // In a real implementation, you would redirect to the Stripe checkout URL
//                string successUrl = Url.Action("PaymentSuccess", "Payment", new { session_id = stripeSessionId }, Request.Scheme);
//                string cancelUrl = Url.Action("PaymentCancel", "Payment", null, Request.Scheme);

//                // Return information needed for frontend to redirect to Stripe
//                return Json(new
//                {
//                    StripeSessionId = stripeSessionId,
//                    SuccessUrl = successUrl,
//                    CancelUrl = cancelUrl
//                });
//            }
//            catch (Exception ex) {
//                return StatusCode(500, new { Message = "An error occurred during payment initiation", Error = ex.Message });
//            }
//        }

//        // GET: Payment/PaymentSuccess
//        [HttpGet]
//        public async Task<IActionResult> PaymentSuccess(string session_id, string payment_intent = null, string payment_intent_client_secret = null) {
//            try {
//                // Get transaction details from Stripe (simplified)
//                string transactionId = "txn_" + Guid.NewGuid().ToString("N");
//                string paymentIntentId = payment_intent ?? "pi_" + Guid.NewGuid().ToString("N");
//                string paymentMethod = "card";

//                // Process successful payment
//                var success = await paymentRepository.ProcessSuccessfulPaymentAsync(
//                    session_id,
//                    transactionId,
//                    paymentIntentId,
//                    paymentMethod
//                );

//                if (!success) {
//                    return RedirectToAction("PaymentError", new { message = "Payment could not be processed" });
//                }

//                // Get purchase details for confirmation
//                var purchase = await paymentRepository.GetPurchaseByStripeSessionIdAsync(session_id);

//                // Return success view
//                return View(purchase);
//            }
//            catch (Exception ex) {
//                return RedirectToAction("PaymentError", new { message = ex.Message });
//            }
//        }

//        // GET: Payment/PaymentCancel
//        [HttpGet]
//        public IActionResult PaymentCancel() {
//            // Payment was canceled by the user
//            return View();
//        }

//        // GET: Payment/PaymentError
//        [HttpGet]
//        public IActionResult PaymentError(string message) {
//            ViewBag.ErrorMessage = message;
//            return View();
//        }
//    }

//    // ViewModel for checkout page
//    public class CheckoutViewModel
//    {
//        public List<CartItem> CartItems { get; set; }
//        public List<Product> Products { get; set; }
//        public Dictionary<int, List<(Product Product, int Quantity)>> ItemsByStore { get; set; }
//    }
////}