using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Base;
using CMS.Ecommerce;
using CMS.Helpers;

using DancingGoat.ActionSelectors;
using DancingGoat.Models.Checkout;
using DancingGoat.Repositories;
using DancingGoat.Services;

namespace DancingGoat.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IShoppingService mShoppingService;
        private readonly ICheckoutService mCheckoutService;
        private readonly IContactRepository mContactRepository;
        private readonly IProductRepository mProductRepository;


        public CheckoutController(IShoppingService shoppingService, IContactRepository contactRepository, IProductRepository productRepository, ICheckoutService checkoutService)
        {
            mShoppingService = shoppingService;
            mContactRepository = contactRepository;
            mCheckoutService = checkoutService;
            mProductRepository = productRepository;
        }


        public ActionResult ShoppingCart()
        {
            var viewModel = mCheckoutService.PrepareCartViewModel();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult ShoppingCartCheckout()
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            var validationErrors = ShoppingCartInfoProvider.ValidateShoppingCart(cart);

            cart.Evaluate();

            if (!validationErrors.Any())
            {
                var customer = GetCustomerOrCreateFromAuthenticatedUser();
                if (customer != null)
                {
                    mShoppingService.SetCustomer(customer);
                }

                mShoppingService.SaveCart();
                return RedirectToAction("DeliveryDetails");
            }

            // Fill model state with errors from the check result
            ProcessCheckResult(validationErrors);

            var viewModel = mCheckoutService.PrepareCartViewModel();

            return View("ShoppingCart", viewModel);
        }


        public ActionResult DeliveryDetails()
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }

            var viewModel = mCheckoutService.PrepareDeliveryDetailsViewModel();

            return View(viewModel);
        }


        public ActionResult PreviewAndPay()
        {
            if (mShoppingService.GetCurrentShoppingCart().IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }

            var viewModel = mCheckoutService.PreparePreviewViewModel();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreviewAndPay(PreviewViewModel model)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            if (cart.IsEmpty)
            {
                ModelState.AddModelError("cart.empty", ResHelper.GetString("DancingGoatMvc.Checkout.EmptyCartError"));

                var viewModel = mCheckoutService.PreparePreviewViewModel(model.PaymentMethod);
                return View("PreviewAndPay", viewModel);
            }

            if (!mCheckoutService.IsPaymentMethodValid(model.PaymentMethod.PaymentMethodID))
            {
                ModelState.AddModelError("PaymentMethod.PaymentMethodID", ResHelper.GetString("DancingGoatMvc.Payment.PaymentMethodRequired"));
            }
            else
            {
                mShoppingService.SetPaymentOption(model.PaymentMethod.PaymentMethodID);
            }

            var validator = new CreateOrderValidator(cart);

            if (!validator.Validate())
            {
                ProcessCheckResult(validator.Errors);
            }

            if (!ModelState.IsValid)
            {
                var viewModel = mCheckoutService.PreparePreviewViewModel(model.PaymentMethod);
                return View("PreviewAndPay", viewModel);
            }

            try
            {
                mShoppingService.CreateOrder();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("cart.createordererror", ex.Message);
                var viewModel = mCheckoutService.PreparePreviewViewModel(model.PaymentMethod);
                return View("PreviewAndPay", viewModel);
            }

            return RedirectToAction("ThankYou");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DeliveryDetails(DeliveryDetailsViewModel model)
        {
            // Check the selected shipping option
            bool isShippingNeeded = mShoppingService.GetCurrentShoppingCart().IsShippingNeeded;
            if (isShippingNeeded && !mCheckoutService.IsShippingOptionValid(model.ShippingOption.ShippingOptionID))
            {
                ModelState.AddModelError("ShippingOption.ShippingOptionID", ResHelper.GetString("DancingGoatMvc.Shipping.ShippingOptionRequired"));
            }

            // Check if the billing address's country and state are valid
            var countryStateViewModel = model.BillingAddress.BillingAddressCountryStateSelector;
            if (!mCheckoutService.IsCountryValid(countryStateViewModel.CountryID))
            {
                countryStateViewModel.CountryID = 0;
                ModelState.AddModelError("BillingAddress.BillingAddressCountryStateSelector.CountryID", ResHelper.GetString("DancingGoatMvc.Address.CountryIsRequired"));
            }
            else if (!mCheckoutService.IsStateValid(countryStateViewModel.CountryID, countryStateViewModel.StateID))
            {
                countryStateViewModel.StateID = 0;
                ModelState.AddModelError("BillingAddress.BillingAddressCountryStateSelector.StateID", ResHelper.GetString("DancingGoatMvc.Address.StateIsRequired"));
            }

            if (!ModelState.IsValid)
            {
                var viewModel = mCheckoutService.PrepareDeliveryDetailsViewModel(model.Customer, model.BillingAddress, model.ShippingOption);

                return View(viewModel);
            }

            var customer = GetCustomerOrCreateFromAuthenticatedUser() ?? new CustomerInfo();
            bool emailCanBeChanged = !User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(customer.CustomerEmail);
            model.Customer.ApplyToCustomer(customer, emailCanBeChanged);
            mShoppingService.SetCustomer(customer);

            var modelAddressID = model.BillingAddress.BillingAddressSelector?.AddressID ?? 0;
            var billingAddress = mCheckoutService.GetAddress(modelAddressID) ?? new AddressInfo();

            model.BillingAddress.ApplyTo(billingAddress);
            billingAddress.AddressPersonalName = $"{customer.CustomerFirstName} {customer.CustomerLastName}";

            mShoppingService.SetBillingAddress(billingAddress);
            mShoppingService.SetShippingOption(model.ShippingOption?.ShippingOptionID ?? 0);

            return RedirectToAction("PreviewAndPay");
        }


        public ActionResult ThankYou()
        {
            var companyContact = mContactRepository.GetCompanyContact();

            var viewModel = new ThankYouViewModel
            {
                Phone = companyContact.Phone
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult AddItem(CartItemUpdateModel item)
        {
            if (ModelState.IsValid)
            {
                mShoppingService.AddItemToCart(item.SKUID, item.Units);
            }

            return RedirectToAction("ShoppingCart");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult UpdateItem(CartItemUpdateModel item)
        {
            if (ModelState.IsValid)
            {
                mShoppingService.UpdateItemQuantity(item.ID, item.Units);
            }
            else
            {
                // Add an item error and save ViewData so that the ShoppingCart action can show validation errors
                var key = item.ID.ToString();
                ModelState.Add(key, ModelState["Units"]);
            }

            var cartViewModel = mCheckoutService.PrepareCartViewModel();
            return View("ShoppingCart", cartViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult RemoveItem(CartItemUpdateModel item)
        {
            mShoppingService.RemoveItemFromCart(item.ID);

            var cartViewModel = mCheckoutService.PrepareCartViewModel();
            return View("ShoppingCart", cartViewModel);
        }


        [HttpPost]
        public JsonResult CustomerAddress(int addressID)
        {
            var address = mCheckoutService.GetCustomerAddress(addressID); 

            if (address == null)
            {
                return null;
            }

            var responseModel = new
            {
                Line1 = address.AddressLine1,
                Line2 = address.AddressLine2,
                City = address.AddressCity,
                PostalCode = address.AddressZip,
                CountryID = address.AddressCountryID,
                StateID = address.AddressStateID,
                PersonalName = address.AddressPersonalName
            };

            return Json(responseModel);
        }


        public ActionResult ItemDetail(int skuId)
        {
            var product = mProductRepository.GetProductForSKU(skuId);

            if (product == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Detail", "Product", new
            {
                guid = product.NodeGUID,
                productAlias = product.NodeAlias
            });
        }


        [HttpPost]
        public JsonResult CountryStates(int countryId)
        {
            var responseModel = mCheckoutService.GetCountryStates(countryId)
                .Select(s => new
                {
                    id = s.StateID,
                    name = HTMLHelper.HTMLEncode(s.StateDisplayName)
                });

            return Json(responseModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentChanged(int? paymentId)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            mShoppingService.SetPaymentOption(paymentId ?? 0);

            var viewModel = new CartViewModel(cart);

            return PartialView("_ShoppingCartTotals", viewModel);
        }


        private void ProcessCheckResult(IEnumerable<IValidationError> validationErrors)
        {
            var itemErrors = validationErrors
                .OfType<ShoppingCartItemValidationError>()
                .GroupBy(g => g.SKUId);

            foreach (var errorGroup in itemErrors)
            {
                var errors = errorGroup
                    .Select(e => e.GetMessage())
                    .Join(" ");

                ModelState.AddModelError(errorGroup.Key.ToString(), errors);
            }
        }


        private CustomerInfo GetCustomerOrCreateFromAuthenticatedUser()
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            var customer = cart.Customer;

            if (customer != null)
            {
                return customer;
            }

            var user = cart.User;

            return user != null ? CustomerHelper.MapToCustomer(user) : null;
        }
    }
}