using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eshop.Repositories
{
    public class PaymentController : Controller
    {
        public class PaymentRepository
        {
            private EshopContext context;

            public PaymentRepository(EshopContext context) {
                this.context = context;
            }

            // Validate cart items and check stock
            public async Task<(bool IsValid, Dictionary<int, string> InvalidItems)> ValidateCartItemsAsync(List<CartItem> cartItems) {
                var invalidItems = new Dictionary<int, string>();

                // Get all products in the cart
                var productIds = cartItems.Select(ci => ci.Id).ToList();
                var products = await this.context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                // Check if all products exist and have sufficient stock
                foreach (var cartItem in cartItems) {
                    var product = products.FirstOrDefault(p => p.Id == cartItem.Id);

                    if (product == null) {
                        invalidItems.Add(cartItem.Id, "Product does not exist");
                    }
                    else if (product.StockQuantity < cartItem.Quantity) {
                        invalidItems.Add(cartItem.Id, $"Insufficient stock. Available: {product.StockQuantity}");
                    }
                }

                return (invalidItems.Count == 0, invalidItems);
            }

            // Get product details for items in cart
            public async Task<List<Product>> GetCartProductsAsync(List<CartItem> cartItems) {
                var productIds = cartItems.Select(ci => ci.Id).ToList();

                return await this.context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();
            }

            // Group cart items by store
            public async Task<Dictionary<int, List<(Product Product, int Quantity)>>> GroupCartItemsByStoreAsync(List<CartItem> cartItems) {
                var products = await GetCartProductsAsync(cartItems);
                var result = new Dictionary<int, List<(Product Product, int Quantity)>>();

                foreach (var product in products) {
                    var cartItem = cartItems.First(ci => ci.Id == product.Id);

                    if (!result.ContainsKey(product.StoreId)) {
                        result[product.StoreId] = new List<(Product, int)>();
                    }

                    result[product.StoreId].Add((product, cartItem.Quantity));
                }

                return result;
            }

            // Create a purchase record when initiating checkout
            public async Task<Purchase> CreatePurchaseAsync(int userId, List<CartItem> cartItems, string stripeSessionId) {
                // Start a transaction
                using var transaction = await this.context.Database.BeginTransactionAsync();

                try {
                    // Get products
                    var products = await GetCartProductsAsync(cartItems);

                    // Calculate total price
                    decimal totalPrice = 0;
                    foreach (var item in cartItems) {
                        var product = products.First(p => p.Id == item.Id);
                        totalPrice += product.Price * item.Quantity;
                    }

                    // Create purchase record
                    var purchase = new Purchase
                    {
                        UserId = userId,
                        TotalPrice = totalPrice,
                        PaymentStatus = "Pending", // Initially set to pending
                        StripeSessionId = stripeSessionId,
                        CreatedAt = DateTime.Now
                    };

                    this.context.Purchases.Add(purchase);
                    await this.context.SaveChangesAsync();

                    // Create purchase items
                    foreach (var item in cartItems) {
                        var product = products.First(p => p.Id == item.Id);

                        var purchaseItem = new PurchaseItem
                        {
                            PurchaseId = purchase.Id,
                            ProductId = item.Id,
                            Quantity = item.Quantity,
                            PriceAtPurchase = product.Price
                        };

                        this.context.PurchaseItems.Add(purchaseItem);
                    }

                    // Group by store and create vendor mappings
                    var itemsByStore = await GroupCartItemsByStoreAsync(cartItems);

                    foreach (var storeGroup in itemsByStore) {
                        int storeId = storeGroup.Key;
                        decimal storeAmount = storeGroup.Value.Sum(item => item.Product.Price * item.Quantity);

                        var vendorMapping = new PurchaseVendorMapping
                        {
                            PurchaseId = purchase.Id,
                            VendorId = storeId,
                            VendorAmount = storeAmount
                        };

                        this.context.PurchaseVendorMappings.Add(vendorMapping);
                    }

                    await this.context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return purchase;
                }
                catch (Exception) {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // Process a successful payment
            public async Task<bool> ProcessSuccessfulPaymentAsync(
                string stripeSessionId,
                string transactionId,
                string paymentIntentId,
                string paymentMethod) {
                // Start a transaction
                using var transaction = await this.context.Database.BeginTransactionAsync();

                try {
                    // Get the purchase record
                    var purchase = await GetPurchaseByStripeSessionIdAsync(stripeSessionId);

                    if (purchase == null) {
                        return false;
                    }

                    // Update purchase status
                    purchase.PaymentStatus = "Completed";
                    this.context.Purchases.Update(purchase);

                    // Create payment record
                    var payment = new Payment
                    {
                        PurchaseId = purchase.Id,
                        UserId = purchase.UserId,
                        PaymentMethod = paymentMethod,
                        TransactionId = transactionId,
                        PaymentIntentId = paymentIntentId,
                        Amount = purchase.TotalPrice,
                        PaymentDate = DateTime.Now
                    };

                    this.context.Payments.Add(payment);

                    // Get purchase items to decrease stock
                    var purchaseItems = await this.context.PurchaseItems
                        .Where(pi => pi.PurchaseId == purchase.Id)
                        .ToListAsync();

                    // Create cart items from purchase items to decrease stock
                    var cartItems = purchaseItems.Select(pi => new CartItem
                    {
                        Id = pi.ProductId,
                        Quantity = pi.Quantity
                    }).ToList();

                    // Decrease product stock
                    await DecreaseProductStockAsync(cartItems);

                    // Create store payouts
                    var vendorMappings = await this.context.PurchaseVendorMappings
                        .Where(vm => vm.PurchaseId == purchase.Id)
                        .ToListAsync();

                    foreach (var vendorMapping in vendorMappings) {
                        var storePayout = new StorePayout
                        {
                            StoreId = vendorMapping.VendorId,
                            PurchaseId = purchase.Id,
                            PayoutAmount = vendorMapping.VendorAmount,
                            PayoutStatus = "Pending",
                            PayoutDate = DateTime.Now,
                            PayoutMethod = "Stripe"
                        };

                        this.context.StorePayouts.Add(storePayout);
                    }

                    await this.context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception) {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // Decrease product stock quantities
            public async Task<bool> DecreaseProductStockAsync(List<CartItem> cartItems) {
                // Start a transaction
                using var transaction = await this.context.Database.BeginTransactionAsync();

                try {
                    // Get products
                    var productIds = cartItems.Select(ci => ci.Id).ToList();
                    var products = await this.context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();

                    // Update stock quantities
                    foreach (var cartItem in cartItems) {
                        var product = products.First(p => p.Id == cartItem.Id);

                        // Double-check stock availability
                        if (product.StockQuantity < cartItem.Quantity) {
                            await transaction.RollbackAsync();
                            return false;
                        }

                        // Decrease stock
                        product.StockQuantity -= cartItem.Quantity;
                        this.context.Products.Update(product);
                    }

                    await this.context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception) {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // Get purchase by Stripe session ID
            public async Task<Purchase> GetPurchaseByStripeSessionIdAsync(string stripeSessionId) {
                return await this.context.Purchases
                    .FirstOrDefaultAsync(p => p.StripeSessionId == stripeSessionId);
            }
        }
    }
}
