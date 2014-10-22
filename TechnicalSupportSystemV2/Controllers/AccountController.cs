using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystem.DAL;
using TechnicalSupportSystem.Models;
using System.Data;
using Postal;

namespace TechnicalSupportSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        private SystemDBContext db = new SystemDBContext();

        /* Password reset and register email code taken and adapted from author Kevin Junghans and account manipulation code part of .NET framework, which includes comments 
         * available at http://kevin-junghans.blogspot.co.uk/2013/04/password-reset-with-simplemembership.html */
        
        [AllowAnonymous] 
        public ActionResult ResetPassword()
        {
            return View(); // loads a view which has a text box for email address.
        }

        [AllowAnonymous]
        public ActionResult ForgotUserName()
        {
            return View();
        }

        /* Method to retrieve username */
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotUserNameEmail(ForgotUserNameModel model)
        {
            UserProfile user = db.UserProfiles.SingleOrDefault(i=>i.Email == model.EmailAddress);
            
            // if username exist then send the username by email
            if(user!=null)
            { 

            dynamic email = new Email("ForgotUsernameEmail");
            email.To = model.EmailAddress; 
            email.UserName =  user.UserName;
            email.Send();
            return View(model);

            }
                // redirect to view
            else
            {
                return RedirectToAction("EmailDoesNotExist");
            }


        }

        // show view to notify user that email doesn't exist
        [AllowAnonymous]
        public ActionResult EmailDoesNotExist()
        {
            return View();
        }

        /* Method to reset password */
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            UserProfile userprofile = db.UserProfiles.SingleOrDefault(i=>i.UserName == model.UserName); // retrieve userprofile
            
            if (userprofile!=null)
            {
                string confirmationToken =
                    WebSecurity.GeneratePasswordResetToken(model.UserName);
                SendPasswordResetEmailConfirmation(userprofile.Email, model.UserName, confirmationToken); // send password reset url

                return RedirectToAction("ResetPwStepTwo"); // 
            }

            return RedirectToAction("InvalidUserName");
        }

        
        [AllowAnonymous]
        public ActionResult ResetPwStepTwo()
        {
            // view which says email sent
            return View();
        }

        [AllowAnonymous]
        public ActionResult InvalidUserName()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation(string Id)
        {
            ResetPasswordConfirmModel model = new ResetPasswordConfirmModel() { Token = Id };
            return View(model);
        }
        

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPasswordConfirmation(ResetPasswordConfirmModel model)
        {
            if (WebSecurity.ResetPassword(model.Token, model.NewPassword))
            {
                return RedirectToAction("PasswordResetSuccess");
            }
            return RedirectToAction("PasswordResetFailure");
        }

       

        [AllowAnonymous]
        public ActionResult PasswordResetFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult PasswordResetSuccess()
        {
            return View();
        }

        /* Method to resend confirmation email*/
        private void ResendConfirmationEmail(string username)
        {
            UserProfile userprofile = db.UserProfiles.SingleOrDefault(i => i.UserName == username);
            string userId= userprofile.UserId.ToString();
            string email = userprofile.Email;

            string query = "select ConfirmationToken from webpages_Membership where UserId ={0}"; // gets the confirmation token
            string token = db.Database.SqlQuery<string>(query, userId).FirstOrDefault();



            SendEmailConfirmation(email, username, token); // sends email
        }

        /* Method to send email about registration*/
        private void SendEmailConfirmation(string to, string username, string confirmationToken)
        {
            dynamic email = new Email("RegEmail");
            email.To = to; 
            email.UserName = username;
            email.ConfirmationToken = confirmationToken;
            email.Send();
        }

        /*Method to send email about password reset*/
        private void SendPasswordResetEmailConfirmation(string to, string username, string confirmationToken)
        {
            dynamic email = new Email("PasswordResetEmail");
            email.To = to;
            email.UserName = username;
            email.ConfirmationToken = confirmationToken;
            email.Send();            
        }




        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        /* Log in validation method */
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            string errorMsg = "The user name or password provided is incorrect."; // show message in view


                if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    UserProfile user = db.UserProfiles.SingleOrDefault(u => u.UserName == model.UserName);
        

                    if (user.IsVerifyed == false)
                    {
                        errorMsg = " You have not been verified by the admin"; // show message in view
                        WebSecurity.Logout(); // log the user out
                    }
                    else
                    {
                        return RedirectToLocal(returnUrl); // else log in 
                    }
                }
                else if ( model.UserName != null && WebSecurity.UserExists(model.UserName) && !WebSecurity.IsConfirmed(model.UserName))
                {                    
                    errorMsg = "You have not completed the registration process. A confirmation email is sent to you";
                    ResendConfirmationEmail(model.UserName); // send email again
                }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", errorMsg);

            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {

            RegisterModel model = new RegisterModel();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Student", Value = "0" });
            items.Add(new SelectListItem { Text = "Supervisor", Value = "1" });
            items.Add(new SelectListItem { Text = "Technician", Value = "2" });
            model.items = items;

            return View(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                // Attempt to register the user

                try
                {
                    string confirmationToken =
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email}, true);
                    
                    SendEmailConfirmation(model.Email, model.UserName, confirmationToken);

                    if (model.SelectedVal.Contains("0"))
                    {
                        UserProfile userProfile = db.UserProfiles.FirstOrDefault(s => s.UserName == model.UserName);
                        Student student = new Student { FirstName = model.FirstName, LastName = model.LastName, MiddleName = model.MiddleName,UserProfile=userProfile };
                        userProfile.UserType = "Student";

                        if (ModelState.IsValid)
                        {
                            db.Entry<UserProfile>(userProfile).State = EntityState.Modified;
                            db.Students.Add(student);
                            db.SaveChanges();
                        }

                    }

                        // if dropdown list for supervisor is selected then do the below
                    else if (model.SelectedVal.Contains("1"))
                    {
                        UserProfile userProfile = db.UserProfiles.FirstOrDefault(s => s.UserName == model.UserName);
                        userProfile.Email= userProfile.Email.ToLower();
                        Supervisor supervisor = new Supervisor { FirstName = model.FirstName, LastName = model.LastName, MiddleName = model.MiddleName, UserProfile = userProfile };
                        userProfile.UserType = "Supervisor";
                        if (ModelState.IsValid)
                        {
                            db.Entry<UserProfile>(userProfile).State = EntityState.Modified;
                            db.Supervisors.Add(supervisor);
                            db.SaveChanges();
                        }

                    }

                    else if (model.SelectedVal.Contains("2"))
                    {
                        UserProfile userProfile = db.UserProfiles.FirstOrDefault(s => s.UserName == model.UserName);
                        Technician technician = new Technician { FirstName = model.FirstName, LastName = model.LastName, MiddleName = model.MiddleName, UserProfile = userProfile };
                        userProfile.UserType = "Technician";
                        if (ModelState.IsValid)
                        {
                            db.Entry<UserProfile>(userProfile).State = EntityState.Modified;
                            db.Technicians.Add(technician);
                            db.SaveChanges();
                        }

                    }

                    


                    return RedirectToAction("RegisterStepTwo", "Account");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // redisplay form, something is wrong if we got this far
            return View(model);
        }

       
        [AllowAnonymous]
        public ActionResult RegisterStepTwo()
        {

            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterConfirmation(String Id)
        {

            if (WebSecurity.ConfirmAccount(Id))
            {
                return RedirectToAction("ConfirmationSuccess");
            }
            return RedirectToAction("ConfirmationFailure");
        }

        [AllowAnonymous]
        public ActionResult ConfirmationSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmationFailure()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (SystemDBContext db = new SystemDBContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
