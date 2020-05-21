using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Helpers;

using DancingGoat.ActionSelectors;
using DancingGoat.Models.Checkout;
using DancingGoat.Services;

namespace DancingGoat.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly IShoppingService mShoppingService;
        private readonly ICheckoutService mCheckoutService;


        public CouponController(IShoppingService shoppingService, ICheckoutService checkoutService)
        {
            mShoppingService = shoppingService;
            mCheckoutService = checkoutService;
        }


        // GET: Coupon/Show
        public ActionResult Show()
        {
            var viewModel = mCheckoutService.PrepareCartViewModel();

            return PartialView("_CouponCodeEdit", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult AddCouponCode(CouponCodesUpdateModel model)
        {
            string couponCode = model.NewCouponCode?.Trim();
            if (string.IsNullOrEmpty(couponCode) || !mShoppingService.AddCouponCode(couponCode))
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { CouponCodeInvalidMessage = ResHelper.GetString("DancingGoatMvc.Checkout.CouponCodeInvalid") }
                };

            }

            return new EmptyResult();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult RemoveCouponCode(CouponCodesUpdateModel model)
        {
            mShoppingService.RemoveCouponCode(model.RemoveCouponCode);

            return new EmptyResult();
        }
    }
}