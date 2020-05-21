using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Helpers;

using DancingGoat.Models.Contacts;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

namespace DancingGoat.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ICafeRepository mCafeRepository;
        private readonly IContactRepository mContactRepository;
        private readonly ICountryRepository mCountryRepository;
        private readonly ISocialLinkRepository mSocialLinkRepository;


        public ContactsController(ICafeRepository cafeRepository, ISocialLinkRepository socialLinkRepository,
            IContactRepository contactRepository, ICountryRepository countryRepository)
        {
            mCountryRepository = countryRepository;
            mCafeRepository = cafeRepository;
            mSocialLinkRepository = socialLinkRepository;
            mContactRepository = contactRepository;
        }


        // GET: Contacts
        public ActionResult Index()
        {
            var documentId = ContactsProvider.GetContacts().Path("/Contacts").Column("DocumentID").TopN(1).GetScalarResult(0);

            if (documentId <= 0)
            {
                return HttpNotFound();
            }

            HttpContext.Kentico().PageBuilder().Initialize(documentId);

            var model = GetIndexViewModel();

            return View(model);
        }


        [ChildActionOnly]
        [ValidateInput(false)]
        public ActionResult CompanyAddress()
        {
            var address = GetCompanyContactModel();

            return PartialView("_Address", address);
        }


        [ChildActionOnly]
        [ValidateInput(false)]
        public ActionResult CompanySocialLinks()
        {
            var socialLinks = mSocialLinkRepository.GetSocialLinks();

            return PartialView("_SocialLinks", socialLinks);
        }


        private IndexViewModel GetIndexViewModel()
        {
            var cafes = mCafeRepository.GetCompanyCafes(4);

            return new IndexViewModel
            {
                CompanyContact = GetCompanyContactModel(),
                CompanyCafes = GetCompanyCafesModel(cafes)
            };
        }


        private ContactModel GetCompanyContactModel()
        {
            return CreateContactModel(mContactRepository.GetCompanyContact());
        }


        private List<ContactModel> GetCompanyCafesModel(IEnumerable<Cafe> cafes)
        {
            return cafes.Select(CreateContactModel).ToList();
        }


        private ContactModel CreateContactModel(IContact contact)
        {
            var countryStateName = CountryStateName.Parse(contact.Country);
            var country = mCountryRepository.GetCountry(countryStateName.CountryName);
            var state = mCountryRepository.GetState(countryStateName.StateName);

            var model = new ContactModel(contact)
            {
                CountryCode = country.CountryTwoLetterCode,
                Country = ResHelper.LocalizeString(country.CountryDisplayName)
            };

            if (state != null)
            {
                model.StateCode = state.StateName;
                model.State = ResHelper.LocalizeString(state.StateDisplayName);
            }

            return model;
        }
    }
}