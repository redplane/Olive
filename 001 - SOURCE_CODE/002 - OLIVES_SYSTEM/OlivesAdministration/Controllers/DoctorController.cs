using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OlivesAdministration.Attributes;
using OlivesAdministration.Models;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Controllers
{
    [Route("api/doctor")]
    public class DoctorController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of DoctorController
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="applicationSetting"></param>
        public DoctorController(IRepositoryAccount repositoryAccount, ILog log, ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _applicationSetting = applicationSetting;
            _log = log;
        }

        #endregion

        /// <summary>
        ///     Access role : Admin
        ///     Description : Retrieve a doctor by using specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve filtered result asynchronously.
            var doctor = await _repositoryAccount.FindDoctorAsync(id, null);

            // No result has been found.
            if (doctor == null)
            {
                // Log error.
                _log.Error($"Cannot find the doctor [Id : {id}]");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Doctor = new
                {
                    doctor.Id,
                    doctor.Person.FirstName,
                    doctor.Person.LastName,
                    doctor.Person.Email,
                    doctor.Person.Password,
                    doctor.Person.Birthday,
                    doctor.Person.Gender,
                    doctor.Person.Address,
                    doctor.Person.Phone,
                    doctor.Person.Role,
                    Photo = InitializeUrl(_applicationSetting.AvatarStorage.Absolute, doctor.Person.Photo, Values.StandardImageExtension),
                    doctor.Rank,
                    Specialty = new
                    {
                        Id = doctor.SpecialtyId,
                        Name = doctor.SpecialtyName
                    },
                    Place = new
                    {
                        Id = doctor.PlaceId,
                        doctor.City,
                        doctor.Country
                    },
                    doctor.Voters,
                    doctor.Money,
                    doctor.Person.Created,
                    doctor.Person.LastModified
                }
            });
        }

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/doctor/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterDoctorViewModel filter)
        {
            // Request parameters haven't been initialized.
            if (filter == null)
            {
                // Initialize the parameters and do validation.
                filter = new FilterDoctorViewModel();
                Validate(filter);
            }

            // Invalid data validation.
            if (!ModelState.IsValid)
            {
                // Log the error.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }
            // Retrieve result from server.
            var result = await _repositoryAccount.FilterDoctorAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Doctors = result.Doctors.Select(x => new
                {
                    x.Id,
                    x.Person.FirstName,
                    x.Person.LastName,
                    x.Person.Email,
                    x.Person.Password,
                    x.Person.Birthday,
                    x.Person.Gender,
                    x.Person.Address,
                    x.Person.Phone,
                    x.Person.Role,
                    Photo = InitializeUrl(_applicationSetting.AvatarStorage.Absolute, x.Person.Photo, Values.StandardImageExtension),
                    x.Rank,
                    Specialty = new
                    {
                        Id = x.SpecialtyId,
                        Name = x.SpecialtyName
                    },
                    Place = new
                    {
                        Id = x.PlaceId,
                        x.City,
                        x.Country
                    },
                    x.Voters,
                    x.Money,
                    x.Person.Created,
                    x.Person.LastModified
                }),
                result.Total
            });
        }

        #region Properties

        /// <summary>
        ///     Instance of repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        /// Instance for logging management.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Class stores application settings information.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

        #endregion
    }
}