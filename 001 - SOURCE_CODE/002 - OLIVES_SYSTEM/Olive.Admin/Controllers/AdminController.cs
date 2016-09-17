using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using log4net;
using Newtonsoft.Json;
using OliveAdmin.Attributes;
using OliveAdmin.Resources;
using OliveAdmin.ViewModels.Edit;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models.Vertexes;
using Shared.ViewModels;
using Shared.ViewModels.Filter;

namespace OliveAdmin.Controllers
{
    [MvcAuthorize(new [] {Role.Admin})]
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
            var accountHashedPassword = _repositoryAccount.FindMd5Password(loginViewModel.Password);

            var filterAdminViewModel = new FilterAccountViewModel();
            filterAdminViewModel.Email = loginViewModel.Email;
            filterAdminViewModel.EmailComparision = TextComparision.Equal;
            filterAdminViewModel.Password = loginViewModel.Password;
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
            if (admin.Status == (byte) AccountStatus.Disabled)
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
        ///     Find a patient by using specific id.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Edit(EditAccountViewModel editAccountViewModel)
        {
            // ModelState is invalid.
            if (!ModelState.IsValid)
                return View(editAccountViewModel);

            // Find account information from controller context.
            var account = (Account)ViewData["Authorization"];

            try
            {
                // Encrypt user password.
                account.Password = _repositoryAccount.FindMd5Password(editAccountViewModel.Password);

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