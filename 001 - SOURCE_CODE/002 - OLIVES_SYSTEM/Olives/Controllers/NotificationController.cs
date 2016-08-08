using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
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
        public async Task<HttpResponseMessage> FilterAppointmentNotificationAsync(
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/notification/seen")]
        public async Task<HttpResponseMessage> MakeAppointmentNotificationSeen([FromUri] int id)
        {
            try
            {
                // Retrieve the request sender.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Find the appointment notification first.
                var filter = new FilterNotificationViewModel();
                filter.Requester = requester.Id;
                filter.Id = id;
                filter.Mode = RecordFilterMode.RequesterIsOwner;
                filter.IsSeen = false;

                // Do the filter.
                var result = await _repositoryNotification.FilterNotificationsAsync(filter);

                // No record has been found.
                if (result.Total != 1)
                {
                    // Log the error.
                    _log.Error($"There (is/are) {result.Total} record(s) (has/have) been found.");

                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Retrieve the first result.
                var notification = result.Notifications.FirstOrDefault();

                // No record has been found.
                if (notification == null)
                {
                    // Log the error.
                    _log.Error($"There (is/are) {result.Total} record(s) (has/have) been found.");

                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Update the seen status.
                notification.IsSeen = true;

                // Update the database.
                await _repositoryNotification.InitializeNotificationAsync(notification);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Notification = new
                    {
                        notification.Id,
                        notification.Type,
                        notification.Topic,
                        notification.Broadcaster,
                        notification.Recipient,
                        notification.Record,
                        notification.Message,
                        notification.Created,
                        notification.IsSeen
                    }
                });
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