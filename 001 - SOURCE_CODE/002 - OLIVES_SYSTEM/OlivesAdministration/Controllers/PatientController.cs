using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OlivesAdministration.Attributes;
using OlivesAdministration.Models;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Controllers
{
    [Route("api/patient")]
    public class PatientController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="applicationSetting"></param>
        public PatientController(IRepositoryAccount repositoryAccount, ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _applicationSetting = applicationSetting;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account DI
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Class stores application settings.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

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
            // Retrieve list of patients.
            var patient = await _repositoryAccount.FindPatientAsync(id, null);

            // No patient has been found.
            if (patient == null)
            {
                // TODO: Add log here.
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
                    patient.Id,
                    patient.Person.Email,
                    patient.Person.Password,
                    patient.Person.FirstName,
                    patient.Person.LastName,
                    patient.Person.Birthday,
                    patient.Person.Phone,
                    patient.Person.Role,
                    patient.Person.Created,
                    patient.Person.LastModified,
                    patient.Person.Gender,
                    patient.Person.Status,
                    patient.Person.Address,
                    patient.Person.Photo,
                    patient.Money,
                    patient.Height,
                    patient.Weight
                }
            });
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

            // Filter patient by using specific conditions.
            var result = await _repositoryAccount.FilterPatientAsync(filter);

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
                    Photo =
                        InitializeUrl(_applicationSetting.AvatarStorage.Relative, x.Person.Photo,
                            Values.StandardImageExtension),
                    x.Money,
                    x.Height,
                    x.Weight
                }),
                result.Total
            });
        }

        #endregion
    }
}