using Eshop.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stripe;
using Stripe.FinancialConnections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace Eshop.Controllers
{
    public class StripeController : Controller
    {

        [HttpPost]
        public ActionResult Create() {
            try {
                var service = new AccountService();

                var options = new AccountCreateOptions
                {
                    Controller = new AccountControllerOptions
                    {
                        StripeDashboard = new AccountControllerStripeDashboardOptions
                        {
                            Type = "express"
                        },
                        Fees = new AccountControllerFeesOptions
                        {
                            Payer = "application"
                        },
                        Losses = new AccountControllerLossesOptions
                        {
                            Payments = "application"
                        },
                    },
                };

                Account account = service.Create(options);

                return Json(new { account = account.Id });
            }
            catch (Exception ex) {
                Console.Write("An error occurred when calling the Stripe API to create an account:  " + ex.Message);
                Response.StatusCode = 500;
                return Json(new { error = ex.Message });
            }
        }

        public IActionResult Checkout()
        {
            return View();
        }

        //        This approach:

        //Creates the store in your database first
        //        Creates the Stripe Express Connect account
        //        Updates the store with the Stripe account ID
        //        Redirects to Stripe's onboarding flow
        //Handles the return/refresh URLs properly

        //The key difference from your sample code is that this connects the Stripe account directly to your store entity and integrates with your existing MVC controllers.

        [HttpPost]
        public async Task<IActionResult> Create(StoreCreateViewModel model) {
            if (ModelState.IsValid) {
                // Create store in your database
                var store = new Store
                {
                    Name = model.Name,
                    OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    // Other store properties
                };

                await _storeRepository.CreateStore(store);

                // Create Stripe connected account
                try {
                    var service = new AccountService();
                    var options = new AccountCreateOptions
                    {
                        Controller = new AccountControllerOptions
                        {
                            StripeDashboard = new AccountControllerStripeDashboardOptions
                            {
                                Type = "express"
                            },
                            Fees = new AccountControllerFeesOptions
                            {
                                Payer = "application"
                            },
                            Losses = new AccountControllerLossesOptions
                            {
                                Payments = "application"
                            },
                        },
                    };

                    Account account = service.Create(options);

                    // Save the Stripe account ID to the store
                    store.StripeConnectId = account.Id;
                    await _storeRepository.UpdateStore(store);

                    // Create account link for onboarding
                    var accountLinkService = new AccountLinkService();
                    var accountLink = accountLinkService.Create(new AccountLinkCreateOptions
                    {
                        Account = account.Id,
                        ReturnUrl = Url.Action("OnboardingComplete", "Stores", new { id = store.Id }, Request.Scheme),
                        RefreshUrl = Url.Action("RefreshOnboarding", "Stores", new { id = store.Id }, Request.Scheme),
                        Type = "account_onboarding",
                    });

                    // Redirect to Stripe onboarding
                    return Redirect(accountLink.Url);
                }
                catch (Exception ex) {
                    // Log error
                    ModelState.AddModelError("", "Error creating Stripe account: " + ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet("Stores/OnboardingComplete/{id}")]
        public async Task<IActionResult> OnboardingComplete(int id) {
            var store = await _storeRepository.GetStoreById(id);
            if (store == null) {
                return NotFound();
            }

            // Verify this user owns the store
            if (store.OwnerId != User.FindFirstValue(ClaimTypes.NameIdentifier)) {
                return Forbid();
            }

            // Show success page or redirect to store dashboard
            return RedirectToAction("Details", new { id = store.Id });
        }

        [HttpGet("Stores/RefreshOnboarding/{id}")]
        public async Task<IActionResult> RefreshOnboarding(int id) {
            var store = await _storeRepository.GetStoreById(id);
            if (store == null) {
                return NotFound();
            }

            // Verify this user owns the store
            if (store.OwnerId != User.FindFirstValue(ClaimTypes.NameIdentifier)) {
                return Forbid();
            }

            // Create a new account link
            var accountLinkService = new AccountLinkService();
            var accountLink = accountLinkService.Create(new AccountLinkCreateOptions
            {
                Account = store.StripeConnectId,
                ReturnUrl = Url.Action("OnboardingComplete", "Stores", new { id = store.Id }, Request.Scheme),
                RefreshUrl = Url.Action("RefreshOnboarding", "Stores", new { id = store.Id }, Request.Scheme),
                Type = "account_onboarding",
            });

            return Redirect(accountLink.Url);
        }

    }
}
