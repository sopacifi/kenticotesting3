using System;
using System.Web;
using System.Web.Mvc;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;

using DancingGoat.Models.Subscription;

using Kentico.Membership;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DancingGoat.Web.Controllers
{
    public class SubscriptionController : Controller
    {
        private SubscribeSettings mNewsletterSubscriptionSettings;

        private readonly ISubscriptionService mSubscriptionService;
        private readonly ISubscriptionApprovalService mSubscriptionApprovalService;
        private readonly IUnsubscriptionProvider mUnsubscriptionProvider;
        private readonly IEmailHashValidator mEmailHashValidator;
        private readonly IContactProvider mContactProvider;

        private SubscribeSettings NewsletterSubscriptionSettings
        {
            get
            {
                return mNewsletterSubscriptionSettings ?? (mNewsletterSubscriptionSettings = new SubscribeSettings
                {
                    AllowOptIn = true,
                    RemoveUnsubscriptionFromNewsletter = true,
                    RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                    SendConfirmationEmail = true
                });
            }
        }

        private UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }

        
        public SubscriptionController(ISubscriptionService subscriptionService, ISubscriptionApprovalService subscriptionApprovalService, IUnsubscriptionProvider unsubscriptionProvider, IEmailHashValidator emailHashValidator, IContactProvider contactProvider)
        {
            mSubscriptionService = subscriptionService;
            mSubscriptionApprovalService = subscriptionApprovalService;
            mUnsubscriptionProvider = unsubscriptionProvider;
            mEmailHashValidator = emailHashValidator;
            mContactProvider = contactProvider;
        }


        // POST: Subscription/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Subscribe(SubscribeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Subscribe", model);
            }

            var newsletter = NewsletterInfoProvider.GetNewsletterInfo("DancingGoatMvcNewsletter", SiteContext.CurrentSiteID);
            var contact = mContactProvider.GetContactForSubscribing(model.Email);

            string resultMessage;
            if (!mSubscriptionService.IsMarketable(contact, newsletter))
            {
                mSubscriptionService.Subscribe(contact, newsletter, NewsletterSubscriptionSettings);
                
                // The subscription service is configured to use double opt-in, but newsletter must allow for it
                resultMessage = ResHelper.GetString(newsletter.NewsletterEnableOptIn ? "DancingGoatMvc.News.ConfirmationSent" : "DancingGoatMvc.News.Subscribed");
            }
            else
            {
                resultMessage = ResHelper.GetString("DancingGoatMvc.News.AlreadySubscribed");
            }
            
            return Content(resultMessage);
        }


        // POST: Subscription/SubscribeAuthenticated
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SubscribeAuthenticated()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Show();
            }

            var user = UserManager.FindByName(User.Identity.Name);
            var contact = mContactProvider.GetContactForSubscribing(user.Email, user.FirstName, user.LastName);
            var newsletter = NewsletterInfoProvider.GetNewsletterInfo("DancingGoatMvcNewsletter", SiteContext.CurrentSiteID);

            string resultMessage;
            if (!mSubscriptionService.IsMarketable(contact, newsletter))
            {
                mSubscriptionService.Subscribe(contact, newsletter, NewsletterSubscriptionSettings);

                // The subscription service is configured to use double opt-in, but newsletter must allow for it
                resultMessage = ResHelper.GetString(newsletter.NewsletterEnableOptIn ? "DancingGoatMvc.News.ConfirmationSent" : "DancingGoatMvc.News.Subscribed");
            }
            else
            {
                resultMessage = ResHelper.GetString("DancingGoatMvc.News.AlreadySubscribed");
            }

            return Content(resultMessage);
        }


        // GET: Subscription/ConfirmSubscription
        [ValidateInput(false)]
        public ActionResult ConfirmSubscription(ConfirmSubscriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidLink"));

                return View(model);
            }

            DateTime parsedDateTime = DateTimeHelper.ZERO_TIME;

            // Parse date and time from query string, if present
            if (!string.IsNullOrEmpty(model.DateTime) && !DateTimeUrlFormatter.TryParse(model.DateTime, out parsedDateTime))
            {
                ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidDateTime"));

                return View(model);
            }

            var result = mSubscriptionApprovalService.ApproveSubscription(model.SubscriptionHash, false, SiteContext.CurrentSiteName, parsedDateTime);

            switch (result)
            {
                case ApprovalResult.Success:
                    model.ConfirmationResult = ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionSucceeded");
                    break;

                case ApprovalResult.AlreadyApproved:
                    model.ConfirmationResult = ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionAlreadyConfirmed");
                    break;

                case ApprovalResult.TimeExceeded:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionTimeExceeded"));
                    break;

                case ApprovalResult.NotFound:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidLink"));
                    break;

                default:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionFailed"));

                    break;
            }

            return View(model);
        }


        // GET: Subscription/Unsubscribe
        [ValidateInput(false)]
        public ActionResult Unsubscribe(UnsubscriptionModel model)
        {
            string invalidUrlMessage = ResHelper.GetString("DancingGoatMvc.News.InvalidUnsubscriptionLink");

            if (ModelState.IsValid)
            {
                bool emailIsValid = mEmailHashValidator.ValidateEmailHash(model.Hash, model.Email);
                
                if (emailIsValid)
                {
                    try
                    {
                        var issue = IssueInfoProvider.GetIssueInfo(model.IssueGuid, SiteContext.CurrentSiteID);

                        if (model.UnsubscribeFromAll)
                        {
                            // Unsubscribes if not already unsubscribed
                            if (!mUnsubscriptionProvider.IsUnsubscribedFromAllNewsletters(model.Email))
                            {
                                mSubscriptionService.UnsubscribeFromAllNewsletters(model.Email, issue?.IssueID);
                            }

                            model.UnsubscriptionResult = ResHelper.GetString("DancingGoatMvc.News.UnsubscribedAll");
                        }
                        else
                        {
                            var newsletter = NewsletterInfoProvider.GetNewsletterInfo(model.NewsletterGuid, SiteContext.CurrentSiteID);
                            if (newsletter != null)
                            {
                                // Unsubscribes if not already unsubscribed
                                if (!mUnsubscriptionProvider.IsUnsubscribedFromSingleNewsletter(model.Email, newsletter.NewsletterID))
                                {
                                    mSubscriptionService.UnsubscribeFromSingleNewsletter(model.Email, newsletter.NewsletterID, issue?.IssueID);
                                }

                                model.UnsubscriptionResult = ResHelper.GetString("DancingGoatMvc.News.Unsubscribed");
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, invalidUrlMessage);
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        ModelState.AddModelError(String.Empty, invalidUrlMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, invalidUrlMessage);
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, invalidUrlMessage);
            }

            return View(model);
        }


        // GET: Subscription/Show
        public ActionResult Show()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Handle authenticated user
                return PartialView("_SubscribeAuthenticated");
            }
            var model = new SubscribeModel();

            return PartialView("_Subscribe", model);
        }
    }
}