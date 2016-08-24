﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using OlivesAdministration.Interfaces;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace OlivesAdministration.Attributes
{
    public class OlivesAuthorize : AuthorizeAttribute
    {
        /// <summary>
        ///     Initialize an instance of OlivesAuthorize attribute with allowed roles.
        /// </summary>
        /// <param name="roles"></param>
        public OlivesAuthorize(Role[] roles)
        {
            Roles = Array.ConvertAll(roles, x => (int) x);
        }

        /// <summary>
        ///     Repository of accound with extended functions.
        /// </summary>
        public IRepositoryAccountExtended RepositoryAccountExtended { get; set; }


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

            // Find the requested password.
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
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = $"{Language.WarnAccountNotLogin}"
                });

                return;
            }

            // Find the md5 hashed password.
            var accountHashedPassword = RepositoryAccountExtended.FindMd5Password(accountPassword);
            
            // Retrieve person whose properties match conditions.
            var person = RepositoryAccountExtended.FindPerson(null, accountEmail, accountHashedPassword, null, null);

            // No person has been found.
            if (person == null)
            {
                // Treat this request is unauthorized.
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = $"{Language.WarnAccountNotLogin}"
                });
                return;
            }

            // Account has been disabled.
            if ((StatusAccount) person.Status == StatusAccount.Inactive)
            {
                // Treat the login isn't successful because of disabled account.
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = $"{Language.WarnDisabledAccount}"
                });

                return;
            }

            // Account is still pending.
            if ((StatusAccount) person.Status == StatusAccount.Pending)
            {
                // Treat the login isn't successful because of pending account.
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Error = $"{Language.WarnPendingAccount}"
                });

                return;
            }

            // Account role isn't enough to access the function.
            if (!Roles.Any(x => x == person.Role))
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnForbiddenAccessMethod}"
                });

            // Store the requester information in action argument.
            actionContext.ActionArguments[HeaderFields.RequestAccountStorage] = person;
        }
    }
}