using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Shared.Constants;
using Shared.Interfaces;

namespace Olives.Attributes
{
    public class OlivesAuthorize : AuthorizeAttribute
    {
        /// <summary>
        ///     Initialize an instance of OlivesAuthorize attribute with allowed roles.
        /// </summary>
        /// <param name="roles"></param>
        public OlivesAuthorize(int[] roles)
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
        public new int[] Roles { get; }

        /// <summary>
        ///     This function is for handling authorization handling.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Retrieve email and password.
            var accountEmail =
                actionContext.Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountEmail, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.Value.FirstOrDefault())
                    .FirstOrDefault();

            var accountPassword =
                actionContext.Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountPassword, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            // Invalid account name or password.
            if (string.IsNullOrEmpty(accountEmail) || string.IsNullOrEmpty(accountPassword))
            {
                // Treat this request is unauthorized.
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            // Retrieve person whose properties match conditions.
            var person = AccountsRepository.FindPerson(accountEmail, accountPassword, null);

            // No person has been found.
            if (person == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            if (!Roles.Any(x => x == person.Role))
            {
                // Role isn't valid. Tell the client the access is forbidden.
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            //base.OnAuthorization(actionContext);
        }
    }
}