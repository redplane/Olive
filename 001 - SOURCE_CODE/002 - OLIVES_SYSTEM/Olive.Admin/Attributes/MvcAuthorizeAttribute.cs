using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using log4net;
using Newtonsoft.Json;
using OliveAdmin.Interfaces;
using Shared.Enumerations;
using Shared.ViewModels;
using Shared.ViewModels.Filter;

namespace OliveAdmin.Attributes
{
    public class MvcAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region Properties

        /// <summary>
        ///     Instance which provides access to account service.
        /// </summary>
        public IRepositoryAccountExtended RepositoryAccountExtended { get; set; }

        /// <summary>
        ///     Repository for logging.
        /// </summary>
        public ILog Log { get; set; }

        /// <summary>
        ///     List of roles can access the function.
        /// </summary>
        private readonly Role[] _roles;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize attribute with default settings.
        /// </summary>
        public MvcAuthorizeAttribute()
        {
        }

        /// <summary>
        ///     Initialize attribute with roles settings.
        /// </summary>
        /// <param name="roles"></param>
        public MvcAuthorizeAttribute(Role[] roles)
        {
            _roles = roles;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for parsing cookie, querying database and decide whether user can access the function or not.
        /// </summary>
        /// <param name="authorizationContext"></param>
        public void OnAuthorization(AuthorizationContext authorizationContext)
        {
            try
            {
                #region Form authentication cookie

                var formAuthenticationCookie =
                    authorizationContext.RequestContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

                // Authentication cookie is invalid.
                if (formAuthenticationCookie == null)
                {
                    if (IsAnonymousAllowed(authorizationContext))
                        return;

                    // Sign the user out to clear the cookie.
                    FormsAuthentication.SignOut();

                    // Treat the request as unauthorized.
                    authorizationContext.Result = new HttpUnauthorizedResult();

                    return;
                }

                // Cookie value is invalid.
                if (string.IsNullOrWhiteSpace(formAuthenticationCookie.Value))
                {
                    if (IsAnonymousAllowed(authorizationContext))
                        return;

                    // Sign the user out to clear the cookie.
                    FormsAuthentication.SignOut();

                    // Treat the request as unauthorized.
                    authorizationContext.Result = new HttpUnauthorizedResult();

                    return;
                }

                #endregion

                #region Form authentication ticket

                // Decrypt the authentication cookie value to authentication ticket instance.
                var formAuthenticationTicket = FormsAuthentication.Decrypt(formAuthenticationCookie.Value);

                // Ticket is invalid.
                if (formAuthenticationTicket == null)
                {
                    if (IsAnonymousAllowed(authorizationContext))
                        return;

                    // Sign the user out to clear the cookie.
                    FormsAuthentication.SignOut();

                    // Treat the request as unauthorized.
                    authorizationContext.Result = new HttpUnauthorizedResult();

                    return;
                }

                // User data is invalid.
                if (string.IsNullOrWhiteSpace(formAuthenticationTicket.UserData))
                {
                    if (IsAnonymousAllowed(authorizationContext))
                        return;

                    // Sign the user out to clear the cookie.
                    FormsAuthentication.SignOut();

                    // Treat the request as unauthorized.
                    authorizationContext.Result = new HttpUnauthorizedResult();

                    return;
                }

                #endregion

                #region Account in database validation

                // Deserialize the user data to the account model.
                var loginViewModel = JsonConvert.DeserializeObject<LoginViewModel>(formAuthenticationTicket.UserData);

                // Invalid account model.
                if (loginViewModel == null)
                {
                    if (IsAnonymousAllowed(authorizationContext))
                        return;

                    // Sign the user out.
                    FormsAuthentication.SignOut();

                    // Treat the request as unauthorized.
                    authorizationContext.Result = new HttpUnauthorizedResult();

                    return;
                }

                // Initialize admin filter.
                var filterAdminViewModel = new FilterAccountViewModel();
                filterAdminViewModel.Email = loginViewModel.Email;
                filterAdminViewModel.EmailComparision = TextComparision.Equal;
                filterAdminViewModel.Password = loginViewModel.Password;
                filterAdminViewModel.PasswordComparision = TextComparision.EqualIgnoreCase;

                // Find the account from database.
                var account = RepositoryAccountExtended.FindAccount(filterAdminViewModel);

                // Account is not found
                if (account == null)
                {
                    if (IsAnonymousAllowed(authorizationContext))
                        return;

                    // Sign the user out.
                    FormsAuthentication.SignOut();

                    // Treat the request as unauthorized.
                    authorizationContext.Result = new HttpUnauthorizedResult();

                    return;
                }

                #endregion

                #region Cookie initialization & response

                // Re-initialize form authentication ticket.
                var authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(formAuthenticationTicket));

                authorizationContext.HttpContext.Response.Cookies.Add(authenticationCookie);

                if (authorizationContext.HttpContext.Session != null)
                {
                    // Clear the password before saved account information to session.
                    loginViewModel.Password = "";

                    // Save the account information to session.
                    authorizationContext.HttpContext.Session["Account"] = account;
                }

                // Role is not allowed to access function.
                if (_roles != null && _roles.Length > 0)
                {
                    if (!_roles.Any(x => x == account.Role))
                    {
                        authorizationContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                        return;
                    }
                }

                // Setup claim identity to HttpContext.
                var claimIdentity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("email", account.Email),
                    new Claim("role", "admin")
                }, "Cookie");

                // Setup claim identity to HttpContext.
                authorizationContext.HttpContext.User = new GenericPrincipal(claimIdentity, new[] {"Admin"});

                // Setup authorization information.
                authorizationContext.Controller.ViewData.Add("Authorization", account);

                #endregion
            }
            catch (Exception exception)
            {
                // Log the exception.
                Log.Error(exception.Message, exception);

                // Sign user out.
                FormsAuthentication.SignOut();

                // Treat the request as unauthorized.
                authorizationContext.Result = new HttpUnauthorizedResult();
            }
        }

        /// <summary>
        ///     Check whether the anonymous is allowed or not.
        /// </summary>
        /// <param name="authorizationContext"></param>
        /// <returns></returns>
        private bool IsAnonymousAllowed(AuthorizationContext authorizationContext)
        {
            return authorizationContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                   authorizationContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                       typeof(AllowAnonymousAttribute), true);
        }

        #endregion
    }
}