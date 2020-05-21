using System.Net;
using System.Threading;
using System.Web.Mvc;

using CMS.ContactManagement;
using CMS.DataProtection;
using CMS.Helpers;

using DancingGoat.Generator;
using DancingGoat.Models.Consent;

namespace DancingGoat.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ICurrentCookieLevelProvider mCookieLevelProvider;
        private readonly IConsentAgreementService mConsentAgreementService;


        public ConsentController(ICurrentCookieLevelProvider cookieLevelProvider, IConsentAgreementService consentAgreementService)
        {
            mCookieLevelProvider = cookieLevelProvider;
            mConsentAgreementService = consentAgreementService;
        }


        // GET: Consent
        public ActionResult Index()
        {
            var consent = ConsentInfoProvider.GetConsentInfo(TrackingConsentGenerator.CONSENT_NAME);
            if (consent != null)
            {
                var model = new ConsentViewModel
                {
                    ConsentShortText = consent.GetConsentText(Thread.CurrentThread.CurrentUICulture.Name).ShortText
                };

                var contact = ContactManagementContext.CurrentContact;
                if ((contact != null) && mConsentAgreementService.IsAgreed(contact, consent))
                {
                    model.IsConsentAgreed = true;
                }

                return PartialView("_TrackingConsent", model);
            }

            return new EmptyResult();
        }


        // POST: Consent/Agree
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Agree()
        {
            var resultStatus = HttpStatusCode.BadRequest;

            var consent = ConsentInfoProvider.GetConsentInfo(TrackingConsentGenerator.CONSENT_NAME);
            if (consent != null)
            {
                mCookieLevelProvider.SetCurrentCookieLevel(CookieLevel.All);
                mConsentAgreementService.Agree(ContactManagementContext.CurrentContact, consent);

                resultStatus = HttpStatusCode.OK;
            }

            // Redirect is handled on client by javascript
            return new HttpStatusCodeResult(resultStatus);
        }
    }
}