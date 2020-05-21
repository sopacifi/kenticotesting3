using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using CMS.Activities.Loggers;
using CMS.ContactManagement;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;

using DancingGoat.Models.Account;

using Kentico.Membership;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace DancingGoat.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMembershipActivityLogger mMembershipActivitiesLogger;


        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }


        public SignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<SignInManager>();
            }
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        public AccountController(IMembershipActivityLogger membershipActivitiesLogger)
        {
            mMembershipActivitiesLogger = membershipActivitiesLogger;
        }


        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var signInResult = SignInStatus.Failure;

            try
            {
                signInResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.StaySignedIn, false);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("AccountController", "Login", ex);
            }

            if (signInResult == SignInStatus.Success)
            {
                ContactManagementContext.UpdateUserLoginContact(model.UserName);

                mMembershipActivitiesLogger.LogLogin(model.UserName);

                var decodedReturnUrl = Server.UrlDecode(returnUrl);
                if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
                {
                    return Redirect(decodedReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(String.Empty, ResHelper.GetString("login_failuretext"));

            return View(model);
        }


        // POST: Account/Logout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }


        // GET: Account/RetrievePassword
        public ActionResult RetrievePassword()
        {
            return PartialView("_RetrievePassword");
        }


        // POST: Account/RetrievePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RetrievePassword(RetrievePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_RetrievePassword", model);
            }

            var user = UserManager.FindByEmail(model.Email);
            if (user != null)
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var url = Url.Action("ResetPassword", "Account", new { userId = user.Id, token }, RequestContext.URL.Scheme);

                await UserManager.SendEmailAsync(user.Id, ResHelper.GetString("DancingGoatMvc.PasswordReset.Email.Subject"), 
                    String.Format(ResHelper.GetString("DancingGoatMvc.PasswordReset.Email.Body"), url));
            }

            return Content(ResHelper.GetString("DancingGoatMvc.PasswordReset.EmailSent"));
        }


        // GET: Account/ResetPassword
        public ActionResult ResetPassword(int? userId, string token)
        {
            if (!userId.HasValue || String.IsNullOrEmpty(token))
            {
                return HttpNotFound();
            }

            if (!VerifyPasswordResetToken(userId.Value, token))
            {
                return View("ResetPasswordInvalidToken");
            }

            var model = new ResetPasswordViewModel()
            {
                UserId = userId.Value,
                Token = token
            };

            return View(model);
        }


        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (ResetUserPassword(model.UserId, model.Token, model.Password).Succeeded)
            {
                return View("ResetPasswordSucceeded");
            }

            return View("ResetPasswordInvalidToken");
        }


        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }


        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.UserName,
                Enabled = true
            };

            var registerResult = new IdentityResult();

            try
            {
                registerResult = await UserManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("AccountController", "Register", ex);
                ModelState.AddModelError(String.Empty, ResHelper.GetString("register_failuretext"));
            }

            if (registerResult.Succeeded)
            {
                mMembershipActivitiesLogger.LogRegistration(model.UserName);
                await SignInManager.SignInAsync(user, true, false);
                ContactManagementContext.UpdateUserLoginContact(model.UserName);

                mMembershipActivitiesLogger.LogLogin(model.UserName);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in registerResult.Errors)
            {
                ModelState.AddModelError(String.Empty, error);
            }

            return View(model);
        }


        // GET: Account/YourAccount
        [Authorize]
        public ActionResult YourAccount()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return View(user);
        }


        // GET: Account/Edit
        [Authorize]
        public ActionResult Edit()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            var model = new PersonalDetailsViewModel(user);
            return View(model);
        }


        // POST: Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(PersonalDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UserName = User.Identity.Name;
                return View(model);
            }

            try
            {
                var user = UserManager.FindByName(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                var userStore = new UserStore(SiteContext.CurrentSiteName);
                await userStore.UpdateAsync(user);

                return RedirectToAction("YourAccount");
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("AccountController", "Edit", ex);
                ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.YourAccount.SaveError"));

                model.UserName = User.Identity.Name;
                return View(model);
            }
        }


        /// <summary>
        /// Verifies if user's password reset token is valid.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="token">Password reset token.</param>
        /// <returns>True if user's password reset token is valid, false when user was not found or token is invalid or has expired.</returns>
        private bool VerifyPasswordResetToken(int userId, string token)
        {
            try
            {
                return UserManager.VerifyUserToken(userId, "ResetPassword", token);
            }
            catch (InvalidOperationException)
            {
                // User with given userId was not found
                return false;
            }
        }


        /// <summary>
        /// Reset user's password.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="token">Password reset token.</param>
        /// <param name="password">New password.</param>
        private IdentityResult ResetUserPassword(int userId, string token, string password)
        {
            try
            {
                return UserManager.ResetPassword(userId, token, password);
            }
            catch (InvalidOperationException)
            {
                // User with given userId was not found
                return IdentityResult.Failed("UserId not found.");
            }
        }
    }
}