using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using log4net;
using Newtonsoft.Json;
using OliveAdmin.Attributes;
using OliveAdmin.Models;
using OliveAdmin.Resources;
using OliveAdmin.ViewModels.Edit;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models.Vertexes;
using Shared.ViewModels;
using Shared.ViewModels.Filter;

namespace OliveAdmin.Controllers
{
    [RoutePrefix("Admin")]
    [MvcAuthorize(new[] { Role.Admin })]
    public class AdminController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="log"></param>
        public AdminController(
            IRepositoryAccount repositoryAccount,
            ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _log = log;
        }

        #endregion

        #region Methods
        
        #region Login & logout

        /// <summary>
        ///     This function is for rendering login page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Requester is already logged in. Redirect him/her to home page.
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        ///     This function is for posting login information to service.
        ///     HttpStatusCode != 200 means login is not successful.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return View(loginViewModel);
            }

            // Requester is already logged in. Redirect him/her to home page.
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            // Find the hashed password from the original one.
            var accountHashedPassword = _repositoryAccount.FindEncryptedPassword(loginViewModel.Password);

            var filterAdminViewModel = new FilterAccountViewModel();
            filterAdminViewModel.Email = loginViewModel.Email;
            filterAdminViewModel.EmailComparision = TextComparision.Equal;
            filterAdminViewModel.Password = _repositoryAccount.FindEncryptedPassword(loginViewModel.Password);
            filterAdminViewModel.PasswordComparision = TextComparision.EqualIgnoreCase;

            // Pass parameter to login function. 
            var admin =
                await
                    _repositoryAccount.FindAccountAsync(filterAdminViewModel);

            // If no result return, that means no account.
            if (admin == null)
            {
                _log.Error($"{loginViewModel.Email} is not found in database");

                // Response to client.
                ModelState.AddModelError("Login",
                    string.Format(Olive_Admin_Language.AccountIsNotFound, loginViewModel.Email));

                return View(loginViewModel);
            }

            // Account is pending.
            if (admin.Status == AccountStatus.Pending)
            {
                _log.Error($"{loginViewModel.Email} is pending");

                // Response to client.
                ModelState.AddModelError("Login",
                    string.Format(Olive_Admin_Language.AccountIsNotFound, loginViewModel.Email));

                return View(loginViewModel);
            }

            // Account is disabled.
            if (admin.Status == (byte)AccountStatus.Disabled)
            {
                _log.Error($"{loginViewModel.Email} is disabled");

                // Response to client.
                ModelState.AddModelError("Login",
                    string.Format(Olive_Admin_Language.AccountIsNotFound, loginViewModel.Email));

                return View(loginViewModel);
            }

            // Update the password with the hashed one
            loginViewModel.Password = accountHashedPassword;

            // Initialize form authentication ticket, encrypt and store it to cookie.
            var formAuthenticationTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName,
                DateTime.Now, DateTime.Now.AddMinutes(30), true, JsonConvert.SerializeObject(loginViewModel));

            // Initialize cookie contain the authorization ticket.
            var httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(formAuthenticationTicket));
            Response.Cookies.Add(httpCookie);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// This function is for sign user out of system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            // Clear the authorization cookie.
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Admin");
        }

        #endregion

        #region Profile

        /// <summary>
        /// This function is for reading requester profile and display profile page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Profile")]
        public async Task<ActionResult> PersonalProfile()
        {
            try
            {
                // Find account from session to know the current accessing user.
                var account = (Account)Session[Constant.MvcAccount];

                // Account information is not found. Redirect user to logout page.
                if (account == null)
                    return RedirectToAction("Logout", "Admin");

                // Find the account from database.
                var filterAccountViewModel = new FilterAccountViewModel();
                filterAccountViewModel.Email = account.Email;
                filterAccountViewModel.EmailComparision = TextComparision.EqualIgnoreCase;
                filterAccountViewModel.Password = account.Password;
                filterAccountViewModel.PasswordComparision = TextComparision.EqualIgnoreCase;

                // Access database repository to retrieve full information of account.
                account = await _repositoryAccount.FindAccountAsync(filterAccountViewModel);

                // Clear sensitive informations before throwing 'em to client.
                account.Password = null;
                return View(account);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Find a patient by using specific id.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PersonalProfile(EditPersonalProfileViewModel editPersonalProfileViewModel)
        {
            // ModelState is invalid.
            if (!ModelState.IsValid)
                return View(editPersonalProfileViewModel);

            try
            {
                // Find account information from session.
                var account = (Account)Session[Constant.MvcAccount];

                // Encrypt user password.
                account.Password = _repositoryAccount.FindEncryptedPassword(editPersonalProfileViewModel.Password);

                // Update the account information.
                account = await _repositoryAccount.InitializeAccountAsync(account);

                var loginViewModel = new LoginViewModel();
                loginViewModel.Email = account.Email;
                loginViewModel.Password = account.Password;

                // As the update is successful, cookie should be updated too.
                // Initialize form authentication ticket, encrypt and store it to cookie.
                var formAuthenticationTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName,
                    DateTime.Now, DateTime.Now.AddMinutes(30), true, JsonConvert.SerializeObject(account));

                // Initialize cookie contain the authorization ticket.
                var httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(formAuthenticationTicket));
                Response.Cookies.Add(httpCookie);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Find lost password

        /// <summary>
        /// This function is for rendering find admin account lost password page.
        /// </summary>
        /// <returns></returns>
        [Route("Forgot")]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult FindLostPassword()
        {
            // If the user is logged in. Redirect him/her to home page.
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            
            return View();
        }


        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Instance for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}