﻿using System;
using System.Web.Profile;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;

namespace Olives.Attributes
{
    public class HubAuthorizeAttribute : AuthorizeAttribute
    {
        #region Properties

        /// <summary>
        /// Repository which provides functions to access account databases.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILog _log;
        
        #endregion

        /// <summary>
        /// Initialize an instance with default settings.
        /// </summary>
        public HubAuthorizeAttribute()
        {
            // Dependencies loader.
            _repositoryAccount = GlobalHost.DependencyResolver.Resolve<IRepositoryAccount>();
            _log = GlobalHost.DependencyResolver.Resolve<ILog>();
        }

        /// <summary>
        /// This function is for content-negotiation.
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
                // Find the account.
                var account = _repositoryAccount.FindPersonAsync(null, email, password, null, StatusAccount.Active).Result;

                // Invalid account. The request is unauthorized.
                if (account == null)
                    return false;
                
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
        /// This function is for identity check.
        /// </summary>
        /// <param name="hubIncomingInvokerContext"></param>
        /// <param name="appliesToMethod"></param>
        /// <returns></returns>
        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            var hubRequest = hubIncomingInvokerContext.Hub.Context.Request;

            // Retrieve information from request.
            var email = hubRequest.QueryString[Values.KeySignalrEmail];
            var password = hubRequest.QueryString[Values.KeySignalrPassword];
            
            try
            {
                // Find the account.
                var account = _repositoryAccount.FindPersonAsync(null, email, password, null, StatusAccount.Active).Result;

                if (account == null)
                    return false;

                return true;
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return false;
            }
        }
    }
}