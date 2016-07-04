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
        /// <param name="emailService"></param>
        public AppointmentController(IRepositoryAccount repositoryAccount, IRepositoryAppointment repositoryAppointment,
            ILog log, IEmailService emailService)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryAppointment = repositoryAppointment;
            _log = log;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Make an appointment request to a target person.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/appointment")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeAppointmentViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid appointment filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] {Language.InvalidRequestParameters}
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid appointment filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Request email & password

            // Account email.
            var accountEmail = Request.Headers.Where(
                x =>
                    !string.IsNullOrEmpty(x.Key) &&
                    x.Key.Equals(HeaderFields.RequestAccountEmail))
                .Select(x => x.Value.FirstOrDefault())
                .FirstOrDefault();

            // Account password.
            var accountPassword = Request.Headers.Where(
                x =>
                    !string.IsNullOrEmpty(x.Key) &&
                    x.Key.Equals(HeaderFields.RequestAccountPassword))
                .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            // Filter person by email & password.
            var person =
                await
                    _repositoryAccount.FindPersonAsync(null, accountEmail, accountPassword, null, StatusAccount.Active);
            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Errors = new[] {Language.WarnNotAuthorizedAccount}
                });
            }

            #endregion

            #region Dater validation

            // Find the dater by using id.
            var dater = await _repositoryAccount.FindPersonAsync(info.Dater, null, null, null, StatusAccount.Active);

            // No information has been found.
            if (dater == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.WarnDaterNotFound}
                });
            }

            // Only patients and doctor can date each other.
            if (dater.Role != (byte) Role.Doctor && dater.Role != (byte) Role.Patient)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Errors = new[] {Language.WarnDaterInvalidRole}
                });
            }

            // 2 people with same role cannot date each other.
            if (dater.Role == person.Role)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Errors = new[] {Language.WarnDaterSameRole}
                });
            }

            #endregion

            // Check whether 2 people have relation with each other or not.
            var isRelationAvailable = await _repositoryAppointment.IsRelationAvailable(person.Id, info.Dater);
            if (!isRelationAvailable)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Errors = new[] {Language.WarnRelationNotExist}
                });
            }

            var appointment = new Appointment();
            appointment.Maker = person.Id;
            appointment.MakerFirstName = person.FirstName;
            appointment.MakerLastName = person.LastName;
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
                        person.Id,
                        person.FirstName,
                        person.LastName
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

            #region Request email & password

            // Account email.
            var accountEmail = Request.Headers.Where(
                x =>
                    !string.IsNullOrEmpty(x.Key) &&
                    x.Key.Equals(HeaderFields.RequestAccountEmail))
                .Select(x => x.Value.FirstOrDefault())
                .FirstOrDefault();

            // Account password.
            var accountPassword = Request.Headers.Where(
                x =>
                    !string.IsNullOrEmpty(x.Key) &&
                    x.Key.Equals(HeaderFields.RequestAccountPassword))
                .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            // Filter person by email & password.
            var person =
                await
                    _repositoryAccount.FindPersonAsync(null, accountEmail, accountPassword, null, StatusAccount.Active);
            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Errors = new[] {Language.WarnNotAuthorizedAccount}
                });
            }

            #endregion

            // Filter appointment by using specific conditions.
            var response = await _repositoryAppointment.FilterAppointmentAsync(filter, accountEmail, accountPassword);
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

        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAppointment _repositoryAppointment;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used for sending emails.
        /// </summary>
        private readonly IEmailService _emailService;

        #endregion
    }
}