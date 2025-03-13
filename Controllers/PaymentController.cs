using Eshop.Extensions;
using Eshop.Models;
using Eshop.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Stripe.Checkout;
using System.Globalization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Eshop.Controllers
{
    public class PaymentController : Controller
    {
        private RepositoryStores repo;
        private const string CartKey = "CartItems";

        public PaymentController(RepositoryStores repoStores) {
            repo = repoStores;
        }

        public async Task<IActionResult> CreateCheckoutSession() {

            List<CartItem> cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartKey);

            if (cartItems == null || cartItems.Count == 0) {
                TempData["Mensaje"] = "Your shopping cart is empty!";
                HttpContext.Session.Remove(CartKey);
                return RedirectToAction("Cart","Cart");
            }


            List<Models.Product> products = await this.repo.GetCartItemsAsync(cartItems);

            var lineItems = new List<SessionLineItemOptions>();

            for (var i = 0; i < products.Count; i++) {
                Models.Product product = products[i];
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = product.Price * 100,
                        Currency = "eur",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name,
                            Description = product.Description
                        }
                    },
                    Quantity = cartItems[i].Quantity,
                });
            }

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentIntentData = new Stripe.Checkout.SessionPaymentIntentDataOptions
                {
                    TransferGroup = "ORDER100",
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7091/Users/Profile?p=1",
                CancelUrl = "https://localhost:7091/Cart/Cart?fail=true"
            };
            var service = new Stripe.Checkout.SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }

        [HttpPost]
        public async Task<IActionResult> Webhook() {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            const string endpointSecret = "whsec_c21";
            try {
                var signatureHeader = Request.Headers["Stripe-Signature"];
                var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);

                // Handle specific checkout session events using the correct event type strings
                switch (stripeEvent.Type) {
                    case "checkout.session.completed":
                        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                        await HandleCheckoutSessionCompleted(session);
                        break;

                    case "checkout.session.async_payment_succeeded":
                        var asyncSucceededSession = stripeEvent.Data.Object as Stripe.Checkout.Session;
                        await HandleCheckoutSessionAsyncPaymentSucceeded(asyncSucceededSession);
                        break;

                    case "checkout.session.async_payment_failed":
                        var asyncFailedSession = stripeEvent.Data.Object as Stripe.Checkout.Session;
                        await HandleCheckoutSessionAsyncPaymentFailed(asyncFailedSession);
                        break;

                    default:
                        Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e) {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
            catch (Exception e) {
                Console.WriteLine("Unexpected error: {0}", e.Message);
                return StatusCode(500);
            }
        }


        private async Task HandleCheckoutSessionCompleted(Stripe.Checkout.Session session) {
            try {
                // This is the most common event you'll handle for successful payments
                Console.WriteLine($"Checkout session completed: {session.Id}");

                // 1. Retrieve the full session details with expanded items
                var sessionService = new Stripe.Checkout.SessionService();
                var fullSession = await sessionService.GetAsync(session.Id, new Stripe.Checkout.SessionGetOptions
                {
                    Expand = new List<string> { "line_items", "payment_intent" }
                });

                // 2. Process the order in your database
                //await ProcessOrder(fullSession);

                //// 3. Create transfers to connected accounts
                //await CreateTransfersToStoreOwners(fullSession);
            }
            catch (Exception ex) {
                // Log error - in a production environment, you might want to use a proper logging framework
                Console.WriteLine($"Error handling checkout session completed: {ex.Message}");
                throw;
            }
        }

        private async Task HandleCheckoutSessionAsyncPaymentSucceeded(Stripe.Checkout.Session session) {
            try {
                // This event is fired when an asynchronous payment method (like bank transfers)
                // completes successfully after the checkout
                Console.WriteLine($"Async payment succeeded for session: {session.Id}");

                // Similar steps as the completed event, as this also represents a successful payment
                var sessionService = new Stripe.Checkout.SessionService();
                var fullSession = await sessionService.GetAsync(session.Id, new Stripe.Checkout.SessionGetOptions
                {
                    Expand = new List<string> { "line_items", "payment_intent" }
                });

                // Process the order
                //await ProcessOrder(fullSession);

                //// Create transfers
                //await CreateTransfersToStoreOwners(fullSession);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error handling async payment succeeded: {ex.Message}");
                throw;
            }
        }

        private async Task HandleCheckoutSessionAsyncPaymentFailed(Stripe.Checkout.Session session) {
            try {
                // This event is fired when an asynchronous payment attempt fails
                Console.WriteLine($"Async payment failed for session: {session.Id}");

                // 1. Retrieve the full session for more details
                var sessionService = new Stripe.Checkout.SessionService();
                var fullSession = await sessionService.GetAsync(session.Id, new Stripe.Checkout.SessionGetOptions
                {
                    Expand = new List<string> { "payment_intent" }
                });

                // 2. Update your order status to failed/cancelled
                //await UpdateOrderAsFailed(fullSession);

                //// 3. Optionally notify the customer about the failed payment
                //await NotifyCustomerAboutFailedPayment(fullSession);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error handling async payment failed: {ex.Message}");
                throw;
            }
        }

        //// Helper method to process the order in your database
        //private async Task ProcessOrder(Stripe.Checkout.Session session) {
        //    // Assuming you store the session ID with the order during checkout creation
        //    // and have an order repository to handle database operations
        //    using (var scope = _serviceScopeFactory.CreateScope()) {
        //        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        //        // Find the order by the session ID
        //        var order = await orderRepository.GetOrderBySessionId(session.Id);
        //        if (order == null) {
        //            throw new Exception($"Order not found for session ID: {session.Id}");
        //        }

        //        // Update order status to paid/completed
        //        order.Status = OrderStatus.Paid;
        //        order.PaymentIntentId = session.PaymentIntentId;
        //        order.PaymentStatus = PaymentStatus.Completed;
        //        order.UpdatedAt = DateTime.UtcNow;

        //        // Save the updated order
        //        await orderRepository.UpdateOrderAsync(order);

        //        // You might want to send order confirmation emails here
        //        await _emailService.SendOrderConfirmationEmail(order);
        //    }
        //}

        //// Helper method to create transfers to connected accounts
        //private async Task CreateTransfersToStoreOwners(Stripe.Checkout.Session session) {
        //    try {
        //        // Get order items and their associated store owners
        //        using (var scope = _serviceScopeFactory.CreateScope()) {
        //            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        //            var storeRepository = scope.ServiceProvider.GetRequiredService<IStoreRepository>();

        //            var order = await orderRepository.GetOrderBySessionId(session.Id);
        //            var orderItems = await orderRepository.GetOrderItemsByOrderId(order.Id);

        //            // Group items by store owner
        //            var itemsByStoreOwner = new Dictionary<string, List<OrderItem>>();
        //            foreach (var item in orderItems) {
        //                var product = await orderRepository.GetProductById(item.ProductId);
        //                var store = await storeRepository.GetStoreById(product.StoreId);

        //                if (!itemsByStoreOwner.ContainsKey(store.StripeConnectedAccountId)) {
        //                    itemsByStoreOwner[store.StripeConnectedAccountId] = new List<OrderItem>();
        //                }

        //                itemsByStoreOwner[store.StripeConnectedAccountId].Add(item);
        //            }

        //            // Create transfers for each store owner
        //            var transferService = new TransferService();
        //            foreach (var storeOwnerPair in itemsByStoreOwner) {
        //                var connectedAccountId = storeOwnerPair.Key;
        //                var items = storeOwnerPair.Value;

        //                // Calculate the total amount for this store owner
        //                var storeTotal = items.Sum(i => i.Price * i.Quantity);

        //                // Calculate your platform fee (e.g., 10%)
        //                var platformFeePercentage = 0.1m;
        //                var platformFee = (int)(storeTotal * platformFeePercentage);
        //                var transferAmount = (int)(storeTotal - platformFee);

        //                // Create a transfer to the connected account
        //                var transferOptions = new TransferCreateOptions
        //                {
        //                    Amount = transferAmount,
        //                    Currency = "usd", // Use your currency
        //                    Destination = connectedAccountId,
        //                    SourceTransaction = session.PaymentIntentId, // Link to the original payment
        //                    TransferGroup = order.Id.ToString() // Optional: Group transfers by order
        //                };

        //                var transfer = await transferService.CreateAsync(transferOptions);

        //                // Log the transfer
        //                Console.WriteLine($"Created transfer of {transferAmount} to {connectedAccountId}: {transfer.Id}");

        //                // Record the transfer in your database
        //                await orderRepository.AddTransferRecord(new TransferRecord
        //                {
        //                    OrderId = order.Id,
        //                    StoreId = items.First().StoreId,
        //                    TransferId = transfer.Id,
        //                    Amount = transferAmount,
        //                    Currency = "usd",
        //                    CreatedAt = DateTime.UtcNow
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex) {
        //        Console.WriteLine($"Error creating transfers: {ex.Message}");
        //        throw;
        //    }
        //}

        //// Helper method to update order status for failed payments
        //private async Task UpdateOrderAsFailed(Stripe.Checkout.Session session) {
        //    using (var scope = _serviceScopeFactory.CreateScope()) {
        //        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        //        var order = await orderRepository.GetOrderBySessionId(session.Id);
        //        if (order != null) {
        //            order.Status = OrderStatus.PaymentFailed;
        //            order.PaymentStatus = PaymentStatus.Failed;
        //            order.UpdatedAt = DateTime.UtcNow;

        //            await orderRepository.UpdateOrderAsync(order);
        //        }
        //    }
        //}

        //// Helper method to notify customers about failed payments
        //private async Task NotifyCustomerAboutFailedPayment(Stripe.Checkout.Session session) {
        //    using (var scope = _serviceScopeFactory.CreateScope()) {
        //        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        //        var order = await orderRepository.GetOrderBySessionId(session.Id);

        //        if (order != null) {
        //            // Send email to customer
        //            await _emailService.SendPaymentFailedEmail(order);
        //        }
        //    }
        //}
    }
}