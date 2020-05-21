using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    public class CartViewModel
    {
        private readonly string currencyFormatString;


        public decimal TotalTax { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal GrandTotal { get; set; }

        public bool IsEmpty { get; set; }
        
        public decimal RemainingAmountForFreeShipping { get; set; }

        public IEnumerable<string> AppliedCouponCodes { get; set; }

        public IEnumerable<CartItemViewModel> CartItems { get; set; }

        public CartViewModel(ShoppingCartInfo cart)
        {
            TotalTax = cart.TotalTax;
            TotalShipping = cart.TotalShipping;
            GrandTotal = cart.GrandTotal;
            currencyFormatString = cart.Currency.CurrencyFormatString;
            IsEmpty = cart.IsEmpty;
            RemainingAmountForFreeShipping = cart.CalculateRemainingAmountForFreeShipping();
            AppliedCouponCodes = cart.CouponCodes.AllAppliedCodes.Select(x => x.Code);

            CartItems = cart.CartItems.Select((cartItemInfo) =>
            {
                return new CartItemViewModel
                {
                    CartItemID = cartItemInfo.CartItemID,
                    CartItemUnits = cartItemInfo.CartItemUnits,
                    SKUID = cartItemInfo.SKUID,
                    SKUImagePath = cartItemInfo.SKU.SKUImagePath,
                    SKUName = cartItemInfo.SKU.SKUName,
                    TotalPrice = cartItemInfo.TotalPrice
                };
            });
        }

        public string FormatPrice(decimal price)
        {
            return String.Format(currencyFormatString, price);
        }
    }
}