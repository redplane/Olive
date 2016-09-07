using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using Olive.Admin.Interfaces;
using Shared.Enumerations;
using Shared.ViewModels;

namespace Olive.Admin.Attributes
{
    public class MvcAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region Properties

        /// <summary>
        /// Instance which provides access to account service.
        /// </summary>
        public IRepositoryAccountExtended RepositoryAccountExtended { get; set; }

        /// <summary>
        /// Legal roles which are allowed to access function.
        /// </summary>
        private readonly Role[] _roles;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initialize authorize attribute with default settings.
        /// </summary>
        public MvcAuthorizeAttribute()
        {
            
        }

        /// <summary>
        /// Initialize authorize attribute with default settings.
        /// </summary>
        /// <param name="roles"></param>
        public MvcAuthorizeAttribute(Role[] roles)
        {
            _roles = roles;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// This function is for parsing cookie, querying database and decide whether user can access the function or not.
        /// </summary>
        /// <param name="authorizationContext"></param>
        public void OnAuthorization(AuthorizationContext authorizationContext)
        {
            try
            {
                #region Form authentication cookie
                
                var formAuthenticationCookie = authorizationContext.RequestContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

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
                
                // Find the account from database.
                var account = RepositoryAccountExtended.FindPerson(null, loginViewModel.Email,
                    loginViewModel.Password, (byte)Role.Admin, null);

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

                var claimIdentity = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim("email", account.Email),
                    new Claim("role", "admin")
                }, "Cookie");

                authorizationContext.HttpContext.User = new GenericPrincipal(claimIdentity, new[] {"Admin"});

                // Role is not allowed.
                if (_roles != null && !_roles.Any(x => x == (Role)account.Role))
                {
                    if (IsAnonymousAllowed(authorizationContext))
                        return;

                    authorizationContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    return;
                }
                
                #endregion
            }
            catch (Exception exception)
            {
                // Sign user out.
                FormsAuthentication.SignOut();

                // Treat the request as unauthorized.
                authorizationContext.Result = new HttpUnauthorizedResult();
            }
        }

        /// <summary>
        /// Check whether the anonymous is allowed or not.
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