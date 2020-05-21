using System;
using System.Web.Mvc;
using System.Linq;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Scheduler;

using DancingGoat.Generator;
using DancingGoat.Models.Generator;

namespace DancingGoat.Controllers
{
    [Authorize]
    public class GeneratorController : Controller
    {
        /// <summary>
        /// First name prefix of contacts generated for sample campaigns.
        /// </summary>
        private const string CONTACT_FIRST_NAME_PREFIX = "GeneratedMvcCampaignContact";


        /// <summary>
        /// Last name prefix of contacts generated for sample campaigns.
        /// </summary>
        private const string CONTACT_LAST_NAME_PREFIX = "GeneratedMvcCampaignContactLastName";


        private const string DATA_PROTECTION_SETTINGS_KEY = "DataProtectionSamplesEnabled";


        public ActionResult Index()
        {
            if (!IsAdmin())
            {
                return UnauthorizedView();
            }

            var model = new IndexViewModel();
            return View(model);
        }


        [HttpGet, ActionName("Generate")]
        public ActionResult GenerateGet()
        {
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate()
        {
            if (!IsAdmin())
            {
                return UnauthorizedView();
            }

            new PersonaGenerator().Generate();

            new ContactGroupGenerator().Generate();

            new CampaignContactsDataGenerator(CONTACT_FIRST_NAME_PREFIX, CONTACT_LAST_NAME_PREFIX).Generate();

            var site = Service.Resolve<ISiteService>().CurrentSite;
            new CampaignDataGenerator(site, CONTACT_FIRST_NAME_PREFIX).Generate();

            new OnlineMarketingDataGenerator().Generate();
            new NewslettersDataGenerator(site).Generate();

            var model = new IndexViewModel
            {
                DisplaySuccessMessage = true
            };

            return View("Index", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateABTestData()
        {
            if (!IsAdmin())
            {
                return UnauthorizedView();
            }

            var model = new IndexViewModel();

            try
            {
                var testGenerator = new ABTestConversionGenerator(Service.Resolve<IABTestManager>());
                testGenerator.StartTestAndGenerateData();

                model.DisplaySuccessMessage = true;
            }
            catch (ABTestConversionGeneratorException ex)
            {
                model.ABTestErrorMessage = ex.DisplayMessage;
            }

            return View("Index", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateDataProtectionDemo()
        {
            if (!IsAdmin())
            {
                return UnauthorizedView();
            }

            var site = Service.Resolve<ISiteService>().CurrentSite;

            new TrackingConsentGenerator(site).Generate();
            new FormConsentGenerator(site).Generate();
            new FormContactGroupGenerator().Generate();

            EnableDataProtectionSamples(site);

            var model = new IndexViewModel
            {
                DisplaySuccessMessage = true
            };

            return View("Index", model);
        }


        private void EnableDataProtectionSamples(ISiteInfo site)
        {
            var dataProtectionSamplesEnabledSettingsKey = SettingsKeyInfoProvider.GetSettingsKeyInfo(DATA_PROTECTION_SETTINGS_KEY);
            if (dataProtectionSamplesEnabledSettingsKey?.KeyValue.ToBoolean(false) ?? false)
            {
                return;
            }

            var keyInfo = new SettingsKeyInfo
            {
                KeyName = DATA_PROTECTION_SETTINGS_KEY,
                KeyDisplayName = DATA_PROTECTION_SETTINGS_KEY,
                KeyType = "boolean",
                KeyValue = "True",
                KeyDefaultValue = "False",
                KeyIsGlobal = true,
                KeyIsHidden = true
            };

            SettingsKeyInfoProvider.SetSettingsKeyInfo(keyInfo);
            EnsureTask(site);
        }


        private static void EnsureTask(ISiteInfo site)
        {
            var currentServerName = WebFarmHelper.ServerName;

            var taskServerName = CoreServices.WebFarm.GetEnabledServerNames().First(serverName => !currentServerName.Equals(serverName, StringComparison.Ordinal));

            TaskInterval interval = new TaskInterval
            {
                StartTime = DateTime.Now,
                Period = SchedulingHelper.PERIOD_ONCE
            };

            var task = new TaskInfo
            {
                TaskAssemblyName = "CMS.DancingGoat.Samples",
                TaskClass = "CMS.DancingGoat.Samples.EnableDataProtectionSampleTask",
                TaskEnabled = true,
                TaskLastResult = string.Empty,
                TaskData = string.Empty,
                TaskDisplayName = "Data protection sample",
                TaskName = "EnableDataProtectionSampleTask",
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

        private bool IsAdmin()
        {
            return MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
        }


        private ActionResult UnauthorizedView()
        {
            var model = new IndexViewModel
            {
                IsAuthorized = false
            };

            return View("Index", model);
        }
    }
}