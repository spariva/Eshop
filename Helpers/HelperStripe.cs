//using Eshop.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore.Migrations;
//using Stripe.Checkout;
//using Stripe.Climate;
//using Stripe;

//namespace Eshop.Helpers
//{

//public class StripeConnectCheckoutService
//{
//    private readonly string _stripeSecretKey;
//    private readonly IStripeClient _stripeClient;
//    private readonly IStoreRepository _storeRepository;
//    private readonly IOrderRepository _orderRepository;

//    public StripeConnectCheckoutService(
//        IConfiguration configuration,
//        IStoreRepository storeRepository,
//        IOrderRepository orderRepository)
//    {
//        _stripeSecretKey = configuration["Stripe:SecretKey"];
//        _stripeClient = new StripeClient(_stripeSecretKey);
//        _storeRepository = storeRepository;
//        _orderRepository = orderRepository;
//    }

//    public async Task<string> CreateCheckoutSession(List<CartItem> cartItems, string customerId)
//    {
//         Group items by store
//        var itemsByStore = cartItems.GroupBy(i => i.StoreId);

//        var lineItems = new List<SessionLineItemOptions>();
//        var paymentIntentData = new SessionPaymentIntentDataOptions
//        {
//             This enables automatic transfers to connected accounts
//            TransferData = new SessionPaymentIntentDataTransferDataOptions
//            {
//                 Destination will be set later in the process
//            },
//            ApplicationFeeAmount = 0 // Will set per-store fees
//        };

//         Create line items for all products
//        foreach (var item in cartItems)
//        {
//            lineItems.Add(new SessionLineItemOptions
//            {
//                PriceData = new SessionLineItemPriceDataOptions
//                {
//                    UnitAmount = (long)(item.Price * 100), // Convert to cents
//                    Currency = "usd",
//                    ProductData = new SessionLineItemPriceDataProductDataOptions
//                    {
//                        Name = item.Name,
//                        Description = item.Description
//                    }
//                },
//                Quantity = item.Quantity
//            });
//        }

//         Calculate amounts per store for transfer_group
//        var transferGroup = Guid.NewGuid().ToString();
//        var orderDetails = new List<OrderTransfer>();

//        foreach (var storeGroup in itemsByStore)
//        {
//            var storeId = storeGroup.Key;
//            var store = await _storeRepository.GetStoreById(storeId);
//            var storeAmount = storeGroup.Sum(item => item.Price * item.Quantity);

//             Store transfer info to be processed after checkout
//            orderDetails.Add(new OrderTransfer
//            {
//                StoreId = storeId,
//                Amount = storeAmount,
//                StripeAccountId = store.StripeConnectId
//            });
//        }

//         Create checkout session
//        var options = new SessionCreateOptions
//        {
//            PaymentMethodTypes = new List<string> { "card" },
//            LineItems = lineItems,
//            Mode = "payment",
//            SuccessUrl = "https://yourdomain.com/checkout/success?session_id={CHECKOUT_SESSION_ID}",
//            CancelUrl = "https://yourdomain.com/checkout/cancel",
//            Metadata = new Dictionary<string, string>
//            {
//                { "CustomerId", customerId },
//                { "TransferGroup", transferGroup }
//            },
//            PaymentIntentData = new SessionPaymentIntentDataOptions
//            {
//                TransferGroup = transferGroup
//            }
//        };

//        var service = new SessionService(_stripeClient);
//        var session = await service.CreateAsync(options);

//         Store order info for processing
//        var order = new Order
//        {
//            CustomerId = customerId,
//            SessionId = session.Id,
//            TotalAmount = cartItems.Sum(i => i.Price * i.Quantity),
//            Status = OrderStatus.Pending,
//            CreatedAt = DateTime.UtcNow,
//            TransferGroup = transferGroup,
//            Transfers = orderDetails
//        };

//        await _orderRepository.CreateOrder(order);

//        return session.Url;
//    }

//     Called by webhook after successful payment
//    public async Task ProcessPaymentTransfers(string paymentIntentId, string transferGroup)
//    {
//        var order = await _orderRepository.GetOrderByTransferGroup(transferGroup);

//         Process each store's portion of the payment
//        foreach (var transfer in order.Transfers)
//        {
//             Your platform fee (e.g., 10%)
//            var platformFeePercent = 0.1m;
//            var amount = transfer.Amount;
//            var platformFee = (long)(amount * platformFeePercent * 100); // Convert to cents
//            var transferAmount = (long)(amount * 100); // Convert to cents

//            var transferOptions = new TransferCreateOptions
//            {
//                Amount = transferAmount - platformFee,
//                Currency = "usd",
//                Destination = transfer.StripeAccountId,
//                TransferGroup = transferGroup,
//                SourceTransaction = paymentIntentId,
//                Description = $"Transfer for order #{order.Id}"
//            };

//            var transferService = new TransferService(_stripeClient);
//            var transferResult = await transferService.CreateAsync(transferOptions);

//            transfer.Transferred = true;
//            transfer.TransferId = transferResult.Id;
//        }

//         Update order status
//        order.Status = OrderStatus.Paid;
//        await _orderRepository.UpdateOrder(order);
//    }
//}

// Webhook handler to process the payment completion
//[ApiController]
//[Route("api/webhook")]
//public class StripeWebhookController : ControllerBase
//{
//    private readonly string _webhookSecret;
//    private readonly StripeConnectCheckoutService _checkoutService;

//    public StripeWebhookController(
//        IConfiguration configuration,
//        StripeConnectCheckoutService checkoutService)
//    {
//        _webhookSecret = configuration["Stripe:WebhookSecret"];
//        _checkoutService = checkoutService;
//    }

//    [HttpPost]
//    public async Task<IActionResult> Index()
//    {
//        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

//        try
//        {
//            var stripeEvent = EventUtility.ConstructEvent(
//                json,
//                Request.Headers["Stripe-Signature"],
//                _webhookSecret
//            );

//             Handle payment completion
//            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
//            {
//                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
//                await _checkoutService.ProcessPaymentTransfers(
//                    paymentIntent.Id,
//                    paymentIntent.TransferGroup
//                );
//            }

//            return Ok();
//        }
//        catch (Exception e)
//        {
//            return BadRequest();
//        }
//    }
//}