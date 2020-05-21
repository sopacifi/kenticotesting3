using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Globalization;

namespace DancingGoat.Models.Checkout
{
    [Bind(Exclude = "Countries")]
    public class BillingAddressViewModel
    {
        [Required]
        [Display(Name = "DancingGoatMvc.Address.Line1")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressLine1 { get; set; }



        [Display(Name = "DancingGoatMvc.Address.Line2")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressLine2 { get; set; }


        [Required]
        [Display(Name = "DancingGoatMvc.Address.City")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressCity { get; set; }


        [Required]
        [Display(Name = "DancingGoatMvc.Address.Zip")]
        [MaxLength(20, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressPostalCode { get; set; }
        

        public CountryStateViewModel BillingAddressCountryStateSelector { get; set; }


        public AddressSelectorViewModel BillingAddressSelector { get; set; }


        public SelectList Countries { get; set; }


        public string BillingAddressState { get; set; }


        public string BillingAddressCountry { get; set; }


        public BillingAddressViewModel()
        {
        }


        public BillingAddressViewModel(AddressInfo address, SelectList countries, SelectList addresses = null)
        {
            if (address != null)
            {
                BillingAddressLine1 = address.AddressLine1;
                BillingAddressLine2 = address.AddressLine2;
                BillingAddressCity = address.AddressCity;
                BillingAddressPostalCode = address.AddressZip;
                BillingAddressState = StateInfoProvider.GetStateInfo(address.AddressStateID)?.StateDisplayName ?? String.Empty;
                BillingAddressCountry = CountryInfoProvider.GetCountryInfo(address.AddressCountryID)?.CountryDisplayName ?? String.Empty;
                Countries = countries;
            }

            BillingAddressCountryStateSelector = new CountryStateViewModel
            {
                Countries = countries,
                CountryID = address?.AddressCountryID ?? 0,
                StateID = address?.AddressStateID ?? 0
            };

            BillingAddressSelector = new AddressSelectorViewModel
            {
                Addresses = addresses,
                AddressID = address?.AddressID ?? 0
            };
        }


        public void ApplyTo(AddressInfo address)
        {
            address.AddressLine1 = BillingAddressLine1;
            address.AddressLine2 = BillingAddressLine2;
            address.AddressCity = BillingAddressCity;
            address.AddressZip = BillingAddressPostalCode;
            address.AddressCountryID = BillingAddressCountryStateSelector.CountryID;
            address.AddressStateID = BillingAddressCountryStateSelector.StateID ?? 0;
        }
    }
}