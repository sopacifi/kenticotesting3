using System;

using CMS.Ecommerce;
using CMS.Globalization;

namespace DancingGoat.Models.Orders
{
    public class OrderAddressViewModel
    {
        public string AddressLine1 { get; set; }


        public string AddressLine2 { get; set; }


        public string AddressCity { get; set; }


        public string AddressPostalCode { get; set; }


        public string AddressState { get; set; }


        public string AddressCountry { get; set; }


        public OrderAddressViewModel()
        {
        }


        public OrderAddressViewModel(OrderAddressInfo address)
        {
            if (address == null)
            {
                return;
            }

            AddressLine1 = address.AddressLine1;
            AddressLine2 = address.AddressLine2;
            AddressCity = address.AddressCity;
            AddressPostalCode = address.AddressZip;
            AddressState = StateInfoProvider.GetStateInfo(address.AddressStateID)?.StateDisplayName ?? String.Empty;
            AddressCountry = CountryInfoProvider.GetCountryInfo(address.AddressCountryID)?.CountryDisplayName ?? String.Empty;
        }
    }
}