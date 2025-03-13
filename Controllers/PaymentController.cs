using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Eshop.Controllers
{
    public class PaymentController : Controller
    {

        
        public async Task<IActionResult> CreateCheckoutSession() {

            //var products = (await _cartService.GetDbCartProducts()).Data;
            var products = new List<string>() {"1" }; //not a stripe product!!<Product>
            var lineItems = new List<SessionLineItemOptions>();

            products.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    //UnitAmountDecimal = product.Price * 100,
                    Currency = "eur",
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "ejemplo",
                        Description = "Desc"
                        //Images = new List<string> { product.ImageUrl }
                    },
                    UnitAmount = 10000,
                },
                Quantity = 1
            }));

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
    }
}