using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Shared.Constants;
using Shared.Interfaces;

namespace OlivesAdministration.Attributes
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

        public override void OnAuthorization(HttpActionContext actionContext)
        {   
            // Retrieve email and password of account.
            var accountEmail =
                actionContext.Request.Headers.GetValues(HeaderFields.RequestAccountEmail).FirstOrDefault();
            var accountPassword = actionContext.Request.Headers.GetValues(HeaderFields.RequestAccountPassword).FirstOrDefault();

            // Invalid account name or password.
            if (string.IsNullOrEmpty(accountEmail) || string.IsNullOrEmpty(accountPassword))
            {
                // Treat this request is unauthorized.
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            // Retrieve person whose properties match conditions.
            var person = AccountsRepository.GetPersonExist(accountEmail, true, accountPassword);

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
                return;
            }

            //base.OnAuthorization(actionContext);
        }
        
    }
}