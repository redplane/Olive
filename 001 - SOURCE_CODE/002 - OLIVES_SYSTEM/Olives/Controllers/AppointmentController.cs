using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Enumerations;
using Olives.Hubs;
using Olives.Interfaces;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Olives.Controllers
{
    public class AppointmentController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="repositoryAppointment"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        /// <param name="notificationService"></param>
        public AppointmentController(IRepositoryAccountExtended repositoryAccountExtended,
            IRepositoryAppointment repositoryAppointment, IRepositoryRelationship repositoryRelation,
            ILog log,
            ITimeService timeService, INotificationService notificationService)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositoryAppointment = repositoryAppointment;
            _repositoryRelation = repositoryRelation;
            _log = log;
            _timeService = timeService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Retrieve appointment by search id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/appointment")]
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FindAppointmentAsync([FromUri] int id)
        {
            try
            {
                #region Appointment validation

                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Filter construction.
                var filter = new FilterAppointmentViewModel();
                filter.Id = id;
                filter.Requester = requester.Id;

                // Find the appointment with specific information.
                var result = await _repositoryAppointment.FilterAppointmentAsync(filter);

                // Result is not unique.
                if (result.Total != 1 || result.Appointments == null)
                {
                    // Log the error.
                    _log.Error($"Appointment [Id: {id}] is not unique. Found {result.Total} result(s)");

                    // Tell the client there is no result returned.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Find the appointment by searching id.
                var appointment = result.Appointments.FirstOrDefault();

                // First element is invalid.
                if (appointment == null)
                {
                    // Log the error.
                    _log.Error($"Appointment [Id: {id}] is not unique. Found {result.Total} result(s)");

                    // Tell the client there is no result returned.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Appointment = new
                    {
                        appointment.Id,
                        Maker = new
                        {
                            Id = appointment.Maker,
                            FirstName = appointment.MakerFirstName,
                            LastName = appointment.MakerLastName
                        },
                        Dater = new
                        {
                            Id = appointment.Dater,
                            FirstName = appointment.DaterFirstName,
                            LastName = appointment.DaterLastName
                        },
                        appointment.From,
                        appointment.To,
                        appointment.Note,
                        appointment.LastModifiedNote,
                        appointment.Created,
                        appointment.LastModified,
                        appointment.Status
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Log the error.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Make an appointment request to a target person.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/appointment")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializeAppointmentAsync([FromBody] InitializeAppointmentViewModel info)
        {
            try
            {
                #region Model validation

                // Model hasn't been initialized.
                if (info == null)
                {
                    info = new InitializeAppointmentViewModel();
                    Validate(info);
                }

                // Invalid model state.
                if (!ModelState.IsValid)
                {
                    _log.Error("Request parameters are invalid. Error sent to client.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
                }

                #endregion

                #region Find the dater

                // Retrieve information of person who sent request.
                var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
                
                if (requester.Id == info.Dater)
                {
                    _log.Error($"Dater [Id: {info.Dater}] is not found as active in system");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnDaterNotFound}"
                    });
                }

                var dater = await _repositoryAccountExtended.FindPersonAsync(info.Dater, null, null, null, StatusAccount.Active);
                if (dater == null)
                {
                    _log.Error($"Dater [Id: {info.Dater}] is not found as active in system");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnDaterNotFound}"
                    });
                }

                #endregion

                #region Relation validation
                
                // Check whether 2 people are connected or not.
                var rPeopleConnected = await _repositoryRelation.IsPeopleConnected(requester.Id, info.Dater);
                if (!rPeopleConnected)
                {
                    _log.Error($"Requester [Id: {requester.Id}] doesn't have any relationships with Dater [Id: {info.Dater}]");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }

                #endregion
                
                #region Appointment initialization

                var unixTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Initialize an appointment information.
                var appointment = new Appointment();
                appointment.Maker = requester.Id;
                appointment.MakerFirstName = requester.FirstName;
                appointment.MakerLastName = requester.LastName;
                appointment.Dater = info.Dater;
                appointment.DaterFirstName = dater.FirstName;
                appointment.DaterLastName = dater.LastName;
                appointment.From = info.From ?? unixTime;
                appointment.To = info.To ?? unixTime;
                appointment.Note = info.Note;
                appointment.Created = unixTime;
                appointment.Status = (byte) StatusAppointment.Pending;

                // Initialize an appointment into database.
                appointment = await _repositoryAppointment.InitializeAppointmentAsync(appointment);

                #endregion

                #region Notification initialization

                var notification = new Notification();
                notification.Type = (byte) NotificationType.Create;
                notification.Topic = (byte) NotificationTopic.Appointment;
                notification.Container = appointment.Id;
                notification.ContainerType = (byte) NotificationTopic.Appointment;
                notification.Broadcaster = requester.Id;
                notification.Recipient = dater.Id;
                notification.Record = appointment.Id;
                notification.Message = string.Format(Language.NotificationAppointmentCreate, requester.FullName,
                    appointment.Note);
                notification.Created = unixTime;

                // Broadcast the notification with fault tolerant.
                await _notificationService.BroadcastNotificationAsync(notification, Hub);

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Appointment = new
                    {
                        appointment.Id,
                        Maker = new
                        {
                            requester.Id,
                            requester.FirstName,
                            requester.LastName
                        },
                        Dater = new
                        {
                            dater.Id,
                            dater.FirstName,
                            dater.LastName
                        },
                        info.From,
                        info.To,
                        info.Note,
                        appointment.Created,
                        appointment.Status
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Exception is thrown, log it for future trace.
                _log.Error(exception.Message, exception);

                // Tell the client server is error.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Make an appointment request to a target person.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/appointment")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> EditAppointmentAsync([FromUri] int id,
            [FromBody] EditAppointmentViewModel info)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (info == null)
            {
                info = new EditAppointmentViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Appointment validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find appointment by using id asynchronously.
            var appointment = await _repositoryAppointment.FindAppointmentAsync(id);

            // Requester doesn't take part in the appointment.
            if (appointment == null || !(requester.Id == appointment.Dater || requester.Id == appointment.Maker))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Appointment is cancelled or done.
            if (appointment.Status == (byte) StatusAppointment.Cancelled ||
                appointment.Status == (byte) StatusAppointment.Done)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Appointment modification

            // Find the last modified time.
            var lastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Initialize an appointment information.
            if (info.From != null) appointment.From = info.From.Value;
            if (info.To != null) appointment.To = info.To.Value;
            if (!string.IsNullOrWhiteSpace(info.Note)) appointment.LastModifiedNote = info.Note;
            if (info.Status != null) appointment.Status = (byte) info.Status;

            // Update the last modified time.
            appointment.LastModified = lastModifiedTime;

            // Update the appointment.
            appointment = await _repositoryAppointment.InitializeAppointmentAsync(appointment);

            #endregion

            #region Notification initialization

            // Who should receive the notification.
            var recipient = requester.Id == appointment.Maker ? appointment.Dater : appointment.Maker;

            var notification = new Notification();
            notification.Type = (byte) NotificationType.Edit;
            notification.Topic = (byte) NotificationTopic.Appointment;
            notification.Container = appointment.Id;
            notification.ContainerType = (byte)NotificationTopic.Appointment;
            notification.Broadcaster = requester.Id;
            notification.Recipient = recipient;
            notification.Record = appointment.Id;
            notification.Message = string.Format(Language.NotificationAppointmentEdit, requester.FullName,
                appointment.LastModifiedNote);
            notification.Created = lastModifiedTime;

            // Broadcast the notification with fault tolerant.
            await _notificationService.BroadcastNotificationAsync(notification, Hub);

            #endregion

            #region Result handling

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Appointment = new
                    {
                        appointment.Id,
                        Maker = new
                        {
                            Id = appointment.Maker,
                            FirstName = appointment.MakerFirstName,
                            LastName = appointment.MakerLastName
                        },
                        Dater = new
                        {
                            Id = appointment.Dater,
                            FirstName = appointment.DaterFirstName,
                            LastName = appointment.DaterLastName
                        },
                        info.From,
                        info.To,
                        appointment.Note,
                        appointment.LastModifiedNote,
                        appointment.Created,
                        appointment.Status
                    }
                });
            }
            catch (Exception exception)
            {
                // As exception happens, it should be logged for future error trace.
                _log.Error(exception.Message, exception);

                // Tell the client there is something wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Filter appointment by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/appointment/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterAppointmentAsync([FromBody] FilterAppointmentViewModel filter)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterAppointmentViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Filter appointment by using specific conditions.
            filter.Requester = requester.Id;
            var response = await _repositoryAppointment.FilterAppointmentAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Appointments = response.Appointments.Select(x => new
                {
                    x.Id,
                    x.Created,
                    Dater = new
                    {
                        Id = x.Dater,
                        FirstName = x.DaterFirstName,
                        LastName = x.DaterLastName
                    },
                    Maker = new
                    {
                        Id = x.Maker,
                        FirstName = x.MakerFirstName,
                        LastName = x.MakerLastName
                    },
                    x.From,
                    x.To,
                    x.LastModified,
                    x.Note,
                    x.LastModifiedNote,
                    x.Status
                }),
                response.Total
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository which provides functions to access account database.
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Repository of appointments
        /// </summary>
        private readonly IRepositoryAppointment _repositoryAppointment;

        /// <summary>
        ///     Service of notification.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelationship _repositoryRelation;

        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}