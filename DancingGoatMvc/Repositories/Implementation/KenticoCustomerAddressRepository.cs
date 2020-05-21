using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Provides CRUD operations for customer addresses.
    /// </summary>
    public class KenticoCustomerAddressRepository : ICustomerAddressRepository
    {
        /// <summary>
        /// Returns a customer's address with the specified identifier.
        /// </summary>
        /// <param name="addressId">Identifier of the customer's address.</param>
        /// <returns>Customer's address with the specified identifier. Returns <c>null</c> if not found.</returns>
        public AddressInfo GetById(int addressId)
        {
            return AddressInfoProvider.GetAddressInfo(addressId);
        }


        /// <summary>
        /// Returns an enumerable collection of a customer's addresses.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <returns>Collection of customer's addresses. See <see cref="AddressInfo"/> for detailed information.</returns>
        public IEnumerable<AddressInfo> GetByCustomerId(int customerId)
        {
            return AddressInfoProvider.GetAddresses(customerId).ToList();
        }


        /// <summary>
        /// Saves a customer's address into the database.
        /// </summary>
        /// <param name="address"><see cref="AddressInfo"/> object representing a customer's address that is inserted.</param>
        public void Upsert(AddressInfo address)
        {
            AddressInfoProvider.SetAddressInfo(address);
        }
    }
}