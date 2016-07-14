using System.Collections.Generic;
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
using Shared.ViewModels;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Controllers
{
    [Route("api/doctor")]
    public class DoctorController : ApiParentController
    {
        #region Properties

        /// <summary>
        ///     Instance of repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///  Class stores application settings information.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of DoctorController
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="applicationSetting"></param>
        public DoctorController(IRepositoryAccount repositoryAccount, ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _applicationSetting = applicationSetting;
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
                        doctor.Person.Photo,
                        doctor.Rank,
                        Specialty = new
                        {
                            Id = doctor.SpecialtyId,
                            Name = doctor.SpecialtyName
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            
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
                     City = new
                     {
                         x.City.Id,
                         x.City.Name
                     },
                     Country = new
                     {
                         x.City.Country.Id,
                         x.City.Country.Name
                     },
                     x.Person.Phone,
                     x.Person.Role,
                     x.Person.Status,
                     Photo = InitializeUrl(_applicationSetting.AvatarStorage.Relative, x.Person.Photo, Values.StandardImageExtension),
                     x.Rank,
                     Specialty = new
                     {
                         Id = x.SpecialtyId,
                         Name = x.SpecialtyName
                     },
                     x.Voters,
                     x.Money,
                     x.Person.Created,
                     x.Person.LastModified
                }),
                result.Total
            });
        }
    }
}