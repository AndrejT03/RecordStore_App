using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecordStore.Domain.DTO;
using RecordStore.Service.Interface;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecordStore.Web.Controllers
{
    [Authorize]
    public class ShoppingCartsController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly StripeSettings _stripeSettings;

        public ShoppingCartsController(IShoppingCartService shoppingCartService, IOptions<StripeSettings> stripeSettings)
        {
            _shoppingCartService = shoppingCartService;
            _stripeSettings = stripeSettings.Value;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool applyPoints = TempData["ApplyPoints"] as bool? ?? false;

            var userShoppingCart = _shoppingCartService.GetByUserIdWithIncludedPrducts(Guid.Parse(userId), applyPoints);

            TempData.Keep("ApplyPoints"); 

            return View(userShoppingCart);
        }

        [HttpPost]
        public IActionResult ApplyLoyaltyPoints(bool apply)
        {
            TempData["ApplyPoints"] = apply;
            return RedirectToAction("Index");
        }

        public IActionResult IncreaseQuantity(Guid? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null || userId == null) return NotFound();
            try { _shoppingCartService.IncreaseQuantityInShoppingCart(Guid.Parse(userId), id.Value); }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction("Index");
        }

        public IActionResult ReduceQuantity(Guid? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null || userId == null) return NotFound();
            try { _shoppingCartService.ReduceQuantityInShoppingCart(Guid.Parse(userId), id.Value); }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(Guid? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try { _shoppingCartService.DeleteProductFromShoppingCart(Guid.Parse(userId), id.Value); }
            catch (Exception ex) { return RedirectToAction("Index", new { error = ex.Message }); }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool applyPoints = TempData["ApplyPoints"] as bool? ?? false;
            var userShoppingCart = _shoppingCartService.GetByUserIdWithIncludedPrducts(Guid.Parse(userId), applyPoints);

            TempData["StripePointsApplied"] = applyPoints;

            Stripe.StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = Url.Action("OrderSuccess", "ShoppingCarts", new { }, Request.Scheme),
                CancelUrl = Url.Action("CancelOrder", "ShoppingCarts", new { }, Request.Scheme),
            };

            if (userShoppingCart.AppliedDiscount > 0)
            {
                options.Discounts = new List<SessionDiscountOptions>
                {
                    new SessionDiscountOptions
                    {
                        Coupon = CreateStripeCoupon(userShoppingCart.AppliedDiscount)
                    }
                };
            }

            foreach (var item in userShoppingCart.Items)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)((item.Price ?? 0m) * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title,
                        },
                    },
                    Quantity = item.Quantity,
                });
            }

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { id = session.Id });
        }

        private string CreateStripeCoupon(decimal amount)
        {
            var couponOptions = new Stripe.CouponCreateOptions
            {
                AmountOff = (long)(amount * 100),
                Currency = "usd",
                Duration = "once",
                Name = $"Loyalty Discount {DateTime.Now.Ticks}"
            };
            var couponService = new Stripe.CouponService();
            Stripe.Coupon coupon = couponService.Create(couponOptions);
            return coupon.Id;
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            bool pointsWereApplied = TempData["StripePointsApplied"] as bool? ?? false;

            try
            {
                var (orderPlaced, emailContent) = await _shoppingCartService.OrderRecords(Guid.Parse(userId), pointsWereApplied);
                if (orderPlaced)
                {
                    return View("OrderSuccess", emailContent);
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment was successful, but could not place order.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred after payment: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult CancelOrder()
        {
            TempData["ErrorMessage"] = "Payment was cancelled.";
            return View();
        }
    }
}