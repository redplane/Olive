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
    public class AppointmentNotificationController : ApiParentController
    {
        #region Constructor

        /// <summary>
        /// Initialize an instance with dependency injections.
        /// </summary>
        /// <param name="repositoryAppointmentNotification"></param>
        /// <param name="log"></param>
        public AppointmentNotificationController(IRepositoryAppointmentNotification repositoryAppointmentNotification, ILog log)
        {
            _repositoryAppointmentNotification = repositoryAppointmentNotification;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Filter appointment by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/appointment/notification/filter")]
        [OlivesAuthorize(new [] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterAppointmentNotificationAsync([FromBody] FilterAppointmentNotificationViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                // Initialize the filter.
                filter = new FilterAppointmentNotificationViewModel();

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
                var result = await _repositoryAppointmentNotification.FilterAppointmentNotificationAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    AppointmentNotifications = result.AppointmentNotifications.Select(x => new
                    {
                        x.Id,
                        x.Type,
                        x.Invoker,
                        x.Recipient,
                        x.Created,
                        Appointment = x.AppointmentId,
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
        /// Make the appointment to be seen.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/appointment/notification/seen")]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> MakeAppointmentNotificationSeen([FromUri] int id)
        {
            try
            {
                // Retrieve the request sender.
                var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Find the appointment notification first.
                var filter = new FilterAppointmentNotificationViewModel();
                filter.Requester = requester.Id;
                filter.Id = id;
                filter.Mode = RecordFilterMode.RequesterIsOwner;
                filter.IsSeen = false;
                
                // Do the filter.
                var result = await _repositoryAppointmentNotification.FilterAppointmentNotificationAsync(filter);

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
                var appointmentNotification = result.AppointmentNotifications.FirstOrDefault();

                // No record has been found.
                if (appointmentNotification == null)
                {
                    // Log the error.
                    _log.Error($"There (is/are) {result.Total} record(s) (has/have) been found.");

                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Update the seen status.
                appointmentNotification.IsSeen = true;

                // Update the database.
                await _repositoryAppointmentNotification.InitializeAppointmentNotificationAsync(appointmentNotification);

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
        /// Instance which provide functions to access Appointment Notification database.
        /// </summary>
        private readonly IRepositoryAppointmentNotification _repositoryAppointmentNotification;

        /// <summary>
        /// Instance provides functions to access logger.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}