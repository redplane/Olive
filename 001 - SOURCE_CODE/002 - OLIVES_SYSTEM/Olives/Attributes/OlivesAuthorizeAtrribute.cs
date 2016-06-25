using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

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
                        x.Key.Equals(HeaderFields.RequestAccountEmail))
                    .Select(x => x.Value.FirstOrDefault())
                    .FirstOrDefault();

            var accountPassword =
                actionContext.Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountPassword))
                    .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();
            
            // Invalid account name or password.
            if (string.IsNullOrEmpty(accountEmail) || string.IsNullOrEmpty(accountPassword))
            {
                // Treat this request is unauthorized.
                var errorCode = $"{Language.WarnAccountNotLogin}";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = new
                    {
                        errorCode
                    }
                });
                
                return;
            }

            // Retrieve person whose properties match conditions.
            var person = AccountsRepository.FindPerson(null, accountEmail, accountPassword, null);

            // No person has been found.
            if (person == null)
            {
                // Treat this request is unauthorized.
                var errorCode = $"{Language.WarnAccountNotLogin}";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = new { errorCode }
                });
                return;
            }
            
            // Account has been disabled.
            if ((AccountStatus)person.Status == AccountStatus.Inactive)
            {
                // Treat the login isn't successful because of disabled account.
                var errorCode = $"{Language.WarnDisabledAccount}";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = new { errorCode }
                });

                return;
            }

            // Account is still pending.
            if ((AccountStatus)person.Status == AccountStatus.Pending)
            {
                // Treat the login isn't successful because of pending account.
                var errorCode = $"{Language.WarnPendingAccount}";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = new { errorCode }
                });

                return;
            }
            
            // Account role isn't enough to access the function.
            if (!Roles.Any(x => x == person.Role))
            {
                // Treat the request is forbidden.
                var errorCode = $"{Language.WarnForbiddenAccessMethod}";

                // Role isn't valid. Tell the client the access is forbidden.
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = new { errorCode }
                });
            }

            // Store the requester information in action argument.
            actionContext.ActionArguments["Account"] = person;
        }
    }
}