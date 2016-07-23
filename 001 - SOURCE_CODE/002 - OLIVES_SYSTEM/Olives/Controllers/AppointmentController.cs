using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    public class AppointmentController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryAppointment"></param>
        /// <param name="log"></param>
        public AppointmentController(IRepositoryAccount repositoryAccount, IRepositoryAppointment repositoryAppointment,
            ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryAppointment = repositoryAppointment;
            _log = log;
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
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the appointment by searching id.
            var appointment = await _repositoryAppointment.FindAppointmentAsync(id);

            // Requester is not the person who takes part in the appointment.
            if (!(requester.Id == appointment.Dater || requester.Id == appointment.Maker))
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });

            // Requester is the dater.
            if (requester.Id == appointment.Dater)
            {
                // Find the activate appointment maker.
                var maker =
                    await _repositoryAccount.FindPersonAsync(appointment.Maker, null, null, null, null);

                // Cannot find the maker.
                if (maker == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Appointment = new
                    {
                        appointment.Id,
                        Maker = new
                        {
                            maker.Id,
                            maker.FirstName,
                            maker.LastName
                        },
                        Dater = new
                        {
                            requester.Id,
                            requester.FirstName,
                            requester.LastName
                        },
                        appointment.From,
                        appointment.To,
                        appointment.Note,
                        appointment.Created,
                        appointment.LastModified,
                        appointment.Status
                    }
                });
            }

            // Find the dater.
            var dater = await _repositoryAccount.FindPersonAsync(appointment.Dater, null, null, null, null);

            // Return the information of appointment.
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
                    appointment.From,
                    appointment.To,
                    appointment.Note,
                    appointment.Created,
                    appointment.Status
                }
            });
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
                    _repositoryAccount.FindRelationshipAsync(requester.Id, info.Dater, (byte) StatusRelation.Active);
            if (relationships == null || relationships.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
                });
            }

            #endregion

            #region Appointment initialization

            // Initialize an appointment information.
            var appointment = new Appointment();
            appointment.Maker = requester.Id;
            appointment.MakerFirstName = requester.FirstName;
            appointment.MakerLastName = requester.LastName;
            appointment.Dater = info.Dater;
            appointment.DaterFirstName = dater.FirstName;
            appointment.DaterLastName = dater.LastName;
            appointment.From = info.From ?? 0;
            appointment.To = info.To ?? 0;
            appointment.Note = info.Note;
            appointment.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
            appointment.Status = (byte) StatusAppointment.Pending;

            #endregion

            var result = await _repositoryAppointment.InitializeAppointment(appointment);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Appointment = new
                {
                    result.Id,
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
            #region Model validation

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

            #region Partner

            // By default, no partner is specified.
            Person partner;

            // Requester is the person who makes appointment.
            if (requester.Id == appointment.Maker)
                partner =
                    await _repositoryAccount.FindPersonAsync(appointment.Maker, null, null, null, StatusAccount.Active);
            else
                partner = await _repositoryAccount.FindPersonAsync(appointment.Maker, null, null, null, null);

            // No partner is found.
            if (partner == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnTargetAccountNotFound}"
                });
            }

            #endregion

            #region Appointment initialization

            // Initialize an appointment information.
            if (info.From != null) appointment.From = info.From.Value;
            if (info.To != null) appointment.To = info.To.Value;
            if (info.Note != null) appointment.Note = info.Note;
            if (info.Status != null) appointment.Status = (byte) info.Status;

            #endregion

            // Update the appointment result.
            var result = await _repositoryAppointment.InitializeAppointment(appointment);

            if (requester.Id == appointment.Maker)
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Appointment = new
                    {
                        result.Id,
                        Maker = new
                        {
                            requester.Id,
                            requester.FirstName,
                            requester.LastName
                        },
                        Dater = new
                        {
                            partner.Id,
                            partner.FirstName,
                            partner.LastName
                        },
                        info.From,
                        info.To,
                        info.Note,
                        appointment.Created,
                        appointment.Status,
                        appointment.LastModified
                    }
                });

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Appointment = new
                {
                    result.Id,
                    Maker = new
                    {
                        partner.Id,
                        partner.FirstName,
                        partner.LastName
                    },
                    Dater = new
                    {
                        requester.Id,
                        requester.FirstName,
                        requester.LastName
                    },
                    info.From,
                    info.To,
                    info.Note,
                    appointment.Created,
                    appointment.Status
                }
            });
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
            var response = await _repositoryAppointment.FilterAppointmentAsync(filter, requester.Id);
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
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAppointment _repositoryAppointment;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}