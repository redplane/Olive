using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Hubs;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    public class AppointmentController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryAppointment"></param>
        /// <param name="repositoryAppointmentNotification"></param>
        /// <param name="repositoryTaskCheckAppointment"></param>
        /// <param name="repositoryRealTimeConnection"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        public AppointmentController(IRepositoryAccount repositoryAccount, IRepositoryAppointment repositoryAppointment,
            IRepositoryAppointmentNotification repositoryAppointmentNotification,
            IRepositoryRealTimeConnection repositoryRealTimeConnection, IRepositoryRelation repositoryRelation,
            ILog log, ITimeService timeService)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryAppointment = repositoryAppointment;
            _repositoryAppointmentNotification = repositoryAppointmentNotification;
            _repositoryRealTimeConnection = repositoryRealTimeConnection;
            _repositoryRelation = repositoryRelation;
            _log = log;
            _timeService = timeService;
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
        public async Task<HttpResponseMessage> FindAppointment([FromUri] int id)
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
        public async Task<HttpResponseMessage> InitializeAppointment([FromBody] InitializeAppointmentViewModel info)
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

                #region Relation validation

                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Find the dater by using id.
                var dater = await _repositoryAccount.FindPersonAsync(info.Dater, null, null, null, StatusAccount.Active);

                // No information has been found.
                if (dater == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnDaterNotFound}"
                    });
                }

                // Only patients and doctor can date each other.
                if (dater.Role != (byte) Role.Doctor && dater.Role != (byte) Role.Patient)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnDaterInvalidRole}"
                    });
                }

                // 2 people with same role cannot date each other.
                if (dater.Role == requester.Role)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnDaterSameRole}"
                    });
                }


                // Check whether 2 people have relation with each other or not.
                var relationships =
                    await
                        _repositoryRelation.FindRelationshipAsync(requester.Id, info.Dater, (byte) StatusRelation.Active);
                if (relationships == null || relationships.Count < 1)
                {
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
                appointment.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                appointment.Status = (byte) StatusAppointment.Pending;

                // Initialize an appointment into database.
                appointment = await _repositoryAppointment.InitializeAppointment(appointment);

                #endregion

                #region Appointment notification initialization

                try
                {
                    // Appointment notification create.
                    // As the notification is created successfully. Notification will be pushed to recipient real time.
                    var appointmentNotification = new AppointmentNotification();
                    appointmentNotification.Type = (byte) AppointmentNotificationType.Created;
                    appointmentNotification.Invoker = requester.Id;
                    appointmentNotification.Recipient = info.Dater;
                    appointmentNotification.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                    appointmentNotification.AppointmentId = appointment.Id;
                    appointmentNotification.IsSeen = false;

                    // Initialize appointment notification.
                    await
                        _repositoryAppointmentNotification.InitializeAppointmentNotificationAsync(
                            appointmentNotification);

                    // As the notification is created successfully. Notification should be sent.
                    var connectionIndexes =
                        await
                            _repositoryRealTimeConnection.FindRealTimeConnectionIndexesAsync(dater.Id, null, null);

                    // Send notification to all connection indexes which have been found.
                    Hub.Clients.Clients(connectionIndexes)
                        .notifyAppointment(requester.Id, appointmentNotification.Created,
                            appointmentNotification.AppointmentId, appointmentNotification.Type);
                }
                catch (Exception exception)
                {
                    // As the notification creation is failed. Continue the function.
                    // Notification can be displayed later by long polling request.
                    _log.Error(exception.Message, exception);
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
        public async Task<HttpResponseMessage> EditAppointment([FromUri] int id,
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

            // Initialize an appointment information.
            if (info.From != null) appointment.From = info.From.Value;
            if (info.To != null) appointment.To = info.To.Value;
            if (!string.IsNullOrWhiteSpace(info.Note)) appointment.LastModifiedNote = info.Note;
            if (info.Status != null) appointment.Status = (byte) info.Status;

            // Update the appointment.
            await _repositoryAppointment.InitializeAppointment(appointment);

            #endregion

            #region Notification initialization

            try
            {
                // Who should receive the notification.
                var recipient = requester.Id == appointment.Maker ? appointment.Dater : appointment.Maker;

                var appointmentNotification = new AppointmentNotification();
                appointmentNotification.AppointmentId = appointment.Id;
                appointmentNotification.Type = (byte) AppointmentNotificationType.Edited;
                appointmentNotification.Recipient = recipient;
                appointmentNotification.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Initialize the notification.
                await _repositoryAppointmentNotification.InitializeAppointmentNotificationAsync(appointmentNotification);

                // As the appointment is modified successfully. Notification should be sent.
                var connectionIndexes =
                    await
                        _repositoryRealTimeConnection.FindRealTimeConnectionIndexesAsync(
                            appointmentNotification.Recipient, null, null);

                // Send notification to all connection indexes which have been found.
                Hub.Clients.Clients(connectionIndexes)
                    .notifyAppointment(requester.Id, appointmentNotification.Created,
                        appointmentNotification.AppointmentId, appointmentNotification.Type);
            }
            catch (Exception exception)
            {
                // Notification creation is failed. Log the error and continue the function because it is not important.
                _log.Error(exception.Message, exception);
            }

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
        public async Task<HttpResponseMessage> FilterAppointment([FromBody] FilterAppointmentViewModel filter)
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
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of appointments
        /// </summary>
        private readonly IRepositoryAppointment _repositoryAppointment;

        /// <summary>
        ///     Repository of appointment notification.
        /// </summary>
        private readonly IRepositoryAppointmentNotification _repositoryAppointmentNotification;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Repository which provides function to access real time connection database.
        /// </summary>
        private readonly IRepositoryRealTimeConnection _repositoryRealTimeConnection;

        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}