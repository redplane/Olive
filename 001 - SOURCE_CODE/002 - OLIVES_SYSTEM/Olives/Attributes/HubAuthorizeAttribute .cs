﻿using System;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Olives.Interfaces;
using Shared.Constants;
using Shared.Enumerations;

namespace Olives.Attributes
{
    public class HubAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        ///     Initialize an instance with default settings.
        /// </summary>
        public HubAuthorizeAttribute()
        {
            // Dependencies loader.
            _repositoryAccountExtended = GlobalHost.DependencyResolver.Resolve<IRepositoryAccountExtended>();
            _log = GlobalHost.DependencyResolver.Resolve<ILog>();
        }

        /// <summary>
        ///     This function is for content-negotiation.
        /// </summary>
        /// <param name="hubDescriptor"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            // Retrieve information from request.
            var email = request.QueryString[Values.KeySignalrEmail];
            var password = request.QueryString[Values.KeySignalrPassword];

            try
            {
                // Empty email or password.
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return false;
                
                // Find the hashed password from the original one.
                var accountHashedPassword = _repositoryAccountExtended.FindMd5Password(password);

                // Find the account.
                var account = _repositoryAccountExtended.FindPerson(null, email, accountHashedPassword, null, StatusAccount.Active);

                // Invalid account. The request is unauthorized.
                if (account == null)
                    return false;

                // Save the account information.
                request.Environment[Values.KeySignalrClient] = account;

                return true;
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return false;
            }
        }

        /// <summary>
        ///     This function is for identity check.
        /// </summary>
        /// <param name="hubIncomingInvokerContext"></param>
        /// <param name="appliesToMethod"></param>
        /// <returns></returns>
        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext,
            bool appliesToMethod)
        {
            var hubRequest = hubIncomingInvokerContext.Hub.Context.Request;

            // Retrieve information from request.
            var email = hubRequest.QueryString[Values.KeySignalrEmail];
            var password = hubRequest.QueryString[Values.KeySignalrPassword];

            try
            {
                // Empty email or password.
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return false;

                // Find the hashed password from the original one.
                var accountHashedPassword = _repositoryAccountExtended.FindMd5Password(password);

                // Find the account.
                var account = _repositoryAccountExtended.FindPerson(null, email, accountHashedPassword, null, StatusAccount.Active);
                
                if (account == null)
                    return false;

                // Save the account information.
                hubRequest.Environment[Values.KeySignalrClient] = account;

                return true;
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return false;
            }
        }

        #region Properties

        /// <summary>
        ///     Repository which provides functions to access account databases.
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Logger instance.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}