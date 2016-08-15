using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OlivesAdministration.Attributes;
using OlivesAdministration.Interfaces;
using OlivesAdministration.ViewModels.Filter;
using Shared.Enumerations;
using Shared.Resources;

namespace OlivesAdministration.Controllers
{
    [Route("api/patient")]
    public class PatientController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="log"></param>
        public PatientController(
            IRepositoryAccountExtended repositoryAccountExtended,
            ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _log = log;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account DI
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Instance which provides functions for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Methods

        /// <summary>
        ///     Find a patient by using a specific id.
        /// </summary>
        /// <param name="id">Id of patient</param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Get(int id)
        {
            #region Result find & handling

            try
            {
                // Retrieve list of patients.
                var account =
                    await
                        _repositoryAccountExtended.FindPersonAsync(id, null, null, (byte) Role.Patient, null);

                // No patient has been found.
                if (account == null)
                {
                    _log.Error($"There is no patient [Id: {id}] in database.");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Respond to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Patient = new
                    {
                        account.Id,
                        account.Email,
                        account.Password,
                        account.FirstName,
                        account.LastName,
                        account.Birthday,
                        account.Phone,
                        account.Role,
                        account.Created,
                        account.LastModified,
                        account.Gender,
                        account.Status,
                        account.Address,
                        Photo = account.PhotoUrl,
                        account.Patient.Height,
                        account.Patient.Weight
                    }
                });
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Filter patient by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/patient/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterPatientViewModel filter)
        {
            #region Request parameters validation

            // Filter hasn't been initialized . Initialize it.
            if (filter == null)
            {
                filter = new FilterPatientViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result handling

            // Filter patient by using specific conditions.
            var result = await _repositoryAccountExtended.FilterPatientsAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Patients = result.Patients.Select(x => new
                {
                    x.Id,
                    x.Person.Email,
                    x.Person.Password,
                    x.Person.FirstName,
                    x.Person.LastName,
                    x.Person.Birthday,
                    x.Person.Phone,
                    x.Person.Role,
                    x.Person.Created,
                    x.Person.LastModified,
                    x.Person.Gender,
                    x.Person.Status,
                    x.Person.Address,
                    Photo = x.Person.PhotoUrl,
                    x.Height,
                    x.Weight
                }),
                result.Total
            });

            #endregion
        }

        #endregion
    }
}