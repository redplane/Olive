using System.Linq;
using System.Net;
using System.Web.Mvc;
using Shared.Constants;
using Shared.Interfaces;

namespace OliveAdministration.Attributes
{
    public class OlivesAuthorize : AuthorizeAttribute
    {
        /// <summary>
        ///     Initialize an instance of OlivesAuthorize attribute with allowed roles.
        /// </summary>
        /// <param name="roles"></param>
        public OlivesAuthorize(byte[] roles)
        {
            Roles = roles;
        }

        /// <summary>
        ///     Repository of accounts.
        /// </summary>
        public IRepositoryAccount AccountsRepository { get; set; }

        /// <summary>
        ///     Which roles can access this function.
        /// </summary>
        public new byte[] Roles { get; }

        /// <summary>
        ///     This event is fired when authorization is processed.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Retrieve email and password of account.
            var accountEmail = filterContext.HttpContext.Request.Headers[HeaderFields.RequestAccountEmail];
            var accountPassword = filterContext.HttpContext.Request.Headers[HeaderFields.RequestAccountPassword];

            // Invalid account name or password.
            if (string.IsNullOrEmpty(accountEmail) || string.IsNullOrEmpty(accountPassword))
            {
                // Treat this request is unauthorized.
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            // Retrieve person whose properties match conditions.
            var person = AccountsRepository.GetPersonExist(accountEmail, true, accountPassword);

            // No person has been found.
            if (person == null)
            {
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            if (!Roles.Any(x => x == person.Role))
            {
                // Role isn't valid. Tell the client the access is forbidden.
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }
    }
}