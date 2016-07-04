using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Initialize;

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
        ///     Make an appointment request to a target person.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/appointment")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeAppointmentViewModel info)
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
                _log.Error("Invalid appointment filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Relation validation

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
            
            // Find the dater by using id.
            var dater = await _repositoryAccount.FindPersonAsync(info.Dater, null, null, null, StatusAccount.Active);

            // No information has been found.
            if (dater == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnDaterNotFound}"
                });
            }

            // Only patients and doctor can date each other.
            if (dater.Role != (byte) Role.Doctor && dater.Role != (byte) Role.Patient)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error =  $"{Language.WarnDaterInvalidRole}"
                });
            }

            // 2 people with same role cannot date each other.
            if (dater.Role == requester.Role)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error =  $"{Language.WarnDaterSameRole}"
                });
            }
            

            // Check whether 2 people have relation with each other or not.
            var relationships = await _repositoryAccount.FindRelationParticipation(requester.Id, info.Dater);
            if (relationships == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error =  $"{Language.WarnRelationNotExist}"
                });
            }

            // No active relation has been found.
            if (relationships.All(x => x.Status != (byte) StatusAccount.Active))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRelationNotExist}"
                });
            }

            #endregion

            // Initialize an appointment information.
            var appointment = new Appointment();
            appointment.Maker = requester.Id;
            appointment.MakerFirstName = requester.FirstName;
            appointment.MakerLastName = requester.LastName;
            appointment.Dater = info.Dater;
            appointment.DaterFirstName = dater.FirstName;
            appointment.DaterLastName = dater.LastName;
            appointment.From = info.From;
            appointment.To = info.To;
            appointment.Note = info.Note;
            appointment.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
            appointment.Status = (byte) StatusAppointment.Pending;

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
        /// Filter appointment by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/appointment/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterAppointmentViewModel filter)
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
                _log.Error("Invalid appointment filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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
                        x.Dater.Id,
                        x.Dater.FirstName,
                        x.Dater.LastName
                    },
                    Maker = new
                    {
                        x.Maker.Id,
                        x.Maker.FirstName,
                        x.Maker.LastName
                    },
                    x.From,
                    x.To,
                    x.LastModified,
                    x.Note,
                    x.Status
                })
            });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Repository which provides functions to access account database.
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