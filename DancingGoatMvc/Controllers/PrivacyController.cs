using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataProtection;
using CMS.Helpers;
using CMS.Membership;
using CMS.Scheduler;

using DancingGoat.Generator;
using DancingGoat.Models.Privacy;

namespace DancingGoat.Controllers
{
    public class PrivacyController : Controller
    {
        private readonly IConsentAgreementService mConsentAgreementService;
        private readonly ICurrentCookieLevelProvider mCookieLevelProvider;
        private readonly ICurrentUserContactProvider mCurrentContactProvider;
        private readonly IWebFarmService mWebFarmService;
        private readonly ISiteService mSiteService;
        private ContactInfo mCurrentContact;

        private const string SUCCESS_RESULT = "success";
        private const string ERROR_RESULT = "error";


        private ContactInfo CurrentContact
        {
            get
            {
                if (mCurrentContact == null)
                {
                    // Try to get contact from cookie
                    mCurrentContact = ContactManagementContext.CurrentContact;

                    // If contact is not found, get the contact for current user regardless of the cookie level set
                    if (mCurrentContact == null)
                    {
                        mCurrentContact = mCurrentContactProvider.GetContactForCurrentUser(MembershipContext.AuthenticatedUser);
                    }
                }

                return mCurrentContact;
            }
        }


        public PrivacyController(IConsentAgreementService consentAgreementService, ICurrentCookieLevelProvider cookieLevelProvider,
            ICurrentUserContactProvider currentContactProvider, IWebFarmService webFarmService, ISiteService siteService)
        {
            mConsentAgreementService = consentAgreementService;
            mCookieLevelProvider = cookieLevelProvider;
            mCurrentContactProvider = currentContactProvider;
            mWebFarmService = webFarmService;
            mSiteService = siteService;
        }


        // GET: Privacy
        public ActionResult Index()
        {
            var model = new PrivacyViewModel();

            if (!IsDemoEnabled())
            {
                model.DemoDisabled = true;
            }
            else if (CurrentContact != null)
            {
                model.Constents = GetAgreedConsentsForCurrentContact();
            }

            model.ShowSavedMessage = TempData[SUCCESS_RESULT] != null;
            model.ShowErrorMessage = TempData[ERROR_RESULT] != null;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revoke(string consentName)
        {
            var consentToRevoke = ConsentInfoProvider.GetConsentInfo(consentName);

            if (consentToRevoke != null && CurrentContact != null)
            {
                mConsentAgreementService.Revoke(CurrentContact, consentToRevoke);

                if (consentName == TrackingConsentGenerator.CONSENT_NAME)
                {
                    mCookieLevelProvider.SetCurrentCookieLevel(mCookieLevelProvider.GetDefaultCookieLevel());
                    ExecuteRevokeTrackingConsentTask(mSiteService.CurrentSite, CurrentContact);
                }

                TempData[SUCCESS_RESULT] = true;
            }
            else
            {
                TempData[ERROR_RESULT] = true;
            }

            return RedirectToAction("Index");
        }


        private IEnumerable<ConsentViewModel> GetAgreedConsentsForCurrentContact()
        {
            return mConsentAgreementService.GetAgreedConsents(CurrentContact)
                .Select(consent => new ConsentViewModel
                {
                    Name = consent.Name,
                    Title = consent.DisplayName,
                    Text = consent.GetConsentText(Thread.CurrentThread.CurrentCulture.Name).ShortText
                });
        }


        private bool IsDemoEnabled()
        {
            return ConsentInfoProvider.GetConsentInfo(TrackingConsentGenerator.CONSENT_NAME) != null;
        }


        private void ExecuteRevokeTrackingConsentTask(ISiteInfo site, ContactInfo contact)
        {
            const string TASK_NAME_PREFIX = "DataProtectionSampleRevokeTrackingConsentTask";
            string taskName = $"{TASK_NAME_PREFIX}_{contact.ContactID}";

            // Do not create same task if already scheduled
            var scheduledTask = TaskInfoProvider.GetTaskInfo(taskName, site.SiteID);
            if (scheduledTask != null)
            {
                return;
            }

            var currentServerName = WebFarmHelper.ServerName;
            var taskServerName = mWebFarmService.GetEnabledServerNames().First(serverName => !currentServerName.Equals(serverName, StringComparison.Ordinal));

            TaskInterval interval = new TaskInterval
            {
                StartTime = DateTime.Now,
                Period = SchedulingHelper.PERIOD_ONCE
            };

            var task = new TaskInfo
            {
                TaskAssemblyName = "CMS.DancingGoat.Samples",
                TaskClass = "CMS.DancingGoat.Samples.RevokeTrackingConsentTask",
                TaskEnabled = true,
                TaskLastResult = string.Empty,
                TaskData = contact.ContactID.ToString(),
                TaskDisplayName = "Data protection sample - Revoke tracking consent",
                TaskName = taskName,
                TaskType = ScheduledTaskTypeEnum.System,
                TaskInterval = SchedulingHelper.EncodeInterval(interval),
                TaskNextRunTime = SchedulingHelper.GetFirstRunTime(interval),
                TaskDeleteAfterLastRun = true,
                TaskRunInSeparateThread = true,
                TaskSiteID = site.SiteID,
                TaskServerName = taskServerName
            };

            TaskInfoProvider.SetTaskInfo(task);
        }
    }
}
