using System.ComponentModel.DataAnnotations;

using CMS.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    public class CustomerViewModel
    {
        [Required]
        [Display(Name = "General.Firstname")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "General.Lastname")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "General.RequireEmail")]
        [Display(Name = "General.EmailAddress")]
        [EmailAddress(ErrorMessage = "General.CorrectEmailFormat")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.News.LongEmail")]
        public string Email { get; set; }


        [Display(Name = "General.Phone")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(26, ErrorMessage = "General.MaxlengthExceeded")]
        public string PhoneNumber { get; set; }


        [Display(Name = "com.companyname")]
        [MaxLength(200, ErrorMessage = "General.MaxlengthExceeded")]
        public string Company { get; set; }


        [Display(Name = "com.customer.organizationid")]
        [MaxLength(50, ErrorMessage = "General.MaxlengthExceeded")]
        public string OrganizationID { get; set; }


        [Display(Name = "com.customer.taxregistrationid")]
        [MaxLength(50, ErrorMessage = "General.MaxlengthExceeded")]
        public string TaxRegistrationID { get; set; }


        public bool IsCompanyAccount { get; set; }


        public CustomerViewModel(CustomerInfo customer)
        {
            if (customer == null)
            {
                return;
            }

            FirstName = customer.CustomerFirstName;
            LastName = customer.CustomerLastName;
            Email = customer.CustomerEmail;
            PhoneNumber = customer.CustomerPhone;
            Company = customer.CustomerCompany;
            OrganizationID = customer.CustomerOrganizationID;
            TaxRegistrationID = customer.CustomerTaxRegistrationID;
            IsCompanyAccount = customer.CustomerHasCompanyInfo;
        }


        public CustomerViewModel()
        {
        }


        public void ApplyToCustomer(CustomerInfo customer, bool emailCanBeChanged)
        {
            customer.CustomerFirstName = FirstName;
            customer.CustomerLastName = LastName;
            customer.CustomerPhone = PhoneNumber;
            customer.CustomerCompany = Company;
            customer.CustomerOrganizationID = OrganizationID;
            customer.CustomerTaxRegistrationID = TaxRegistrationID;

            if (emailCanBeChanged)
            {
                customer.CustomerEmail = Email;
            }
        }
    }
}