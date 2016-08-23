﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Models;

namespace Olives.Controllers
{
    [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
    public class NotificationController : ApiParentController
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance with dependency injections.
        /// </summary>
        /// <param name="repositoryNotification"></param>
        /// <param name="log"></param>
        public NotificationController(IRepositoryNotification repositoryNotification,
            ILog log)
        {
            _repositoryNotification = repositoryNotification;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Filter appointment by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/notification/filter")]
        public async Task<HttpResponseMessage> FilterNotificationAsync(
            [FromBody] FilterNotificationViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                // Initialize the filter.
                filter = new FilterNotificationViewModel();

                // Do validation manually.
                Validate(filter);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error for future chase.
                _log.Error("Request parameters are invalid. Errors sent to client");

                // Tell the client the error properties.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Filtering

            try
            {
                // Retrieve the request sender.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Update the filter.
                filter.Requester = requester.Id;

                // Do the filter.
                var result = await _repositoryNotification.FilterNotificationsAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Notifications = result.Notifications.Select(x => new
                    {
                        x.Id,
                        x.Type,
                        x.Topic,
                        x.Container,
                        x.ContainerType,
                        x.Broadcaster,
                        x.Recipient,
                        x.Record,
                        x.Message,
                        x.Created,
                        x.IsSeen
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // Log the exception for future trace.
                _log.Error(exception.Message, exception);

                // Tell the client server has errors.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Make the appointment to be seen.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/notification/seen")]
        public async Task<HttpResponseMessage> MakeNotificationsSeen([FromBody] FilterNotificationViewModel filter)
        {
            try
            {
                // Retrieve the request sender.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Find the appointment notification first.
                filter.Requester = requester.Id;
                filter.Mode = RecordFilterMode.RequesterIsOwner;
                filter.IsSeen = false;

                // Do the filter.
                await _repositoryNotification.ConfirmNotificationSeen(filter);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Log the error.
                _log.Error(exception.Message, exception);

                // Tell the client there is something wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance which provide functions to access Appointment Notification database.
        /// </summary>
        private readonly IRepositoryNotification _repositoryNotification;

        /// <summary>
        ///     Instance provides functions to access logger.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}