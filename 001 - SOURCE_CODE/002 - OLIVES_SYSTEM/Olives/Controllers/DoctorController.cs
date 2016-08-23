using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
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
    [Route("api/doctor")]
    public class DoctorController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="repositorySpecialty"></param>
        /// <param name="repositoryPlace"></param>
        /// <param name="repositoryStorage"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        public DoctorController(IRepositoryAccountExtended repositoryAccountExtended,
            IRepositorySpecialty repositorySpecialty,
            IRepositoryPlace repositoryPlace,
            IRepositoryStorage repositoryStorage,
            ILog log, ITimeService timeService)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositorySpecialty = repositorySpecialty;
            _repositoryPlace = repositoryPlace;
            _repositoryStorage = repositoryStorage;
            _log = log;
            _timeService = timeService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a doctor by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> FindDoctorAsync([FromUri] int id)
        {
            #region Result find

            // Find the doctor by using id.
            var account =
                await
                    _repositoryAccountExtended.FindPersonAsync(id, null, null, (byte) Role.Doctor, StatusAccount.Active);

            // Doctor is not found.
            if (account == null)
            {
                _log.Error($"There is no doctor [Id: {id}] is found as active in database");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result handling

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Doctor = new
                {
                    account.Id,
                    account.FirstName,
                    account.LastName,
                    account.Email,
                    account.Birthday,
                    account.Gender,
                    account.Address,
                    account.Phone,
                    account.Role,
                    Photo = account.PhotoUrl,
                    Specialty = new
                    {
                        account.Doctor.Specialty.Id,
                        account.Doctor.Specialty.Name
                    },
                    Place = new
                    {
                        account.Doctor.Place.Id,
                        account.Doctor.Place.City,
                        account.Doctor.Place.Country
                    },
                    account.Doctor.Rank,
                    account.Doctor.Voters
                }
            });

            #endregion
        }

        /// <summary>
        ///     Sign up as a doctor asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> InitializeDoctorAsync([FromBody] InitializeDoctorViewModel initializer)
        {
            #region Request parameters validation

            // Information hasn't been initialize.
            if (initializer == null)
            {
                // Initialize the default instance and do the validation.
                initializer = new InitializeDoctorViewModel();
                Validate(initializer);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Find the list of speciaties match with searched condition.
            var specialty = await _repositorySpecialty.FindSpecialtyAsync(initializer.Specialty);

            // Invalid results set.
            if (specialty == null)
            {
                // Log the error.
                _log.Error($"Specialty[Id : {initializer.Specialty}] is not found.");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnSpecialtyNotFound}"
                });
            }

            // Find the place by using id.
            var place = await _repositoryPlace.FindPlaceAsync(initializer.Place, null, null, null, null);

            // Place is not found.
            if (place == null)
            {
                // Log the error.
                _log.Error($"Place [Id : {initializer.Place}] is not found.");

                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnPlaceNotFound}"
                });
            }

            #endregion

            #region Information initialization

            #region File initialization

            // Find the storage of profile.
            var fileName = $"{Guid.NewGuid()}.pdf";
            var storageProfile = _repositoryStorage.FindStorage("Profile");
            var profileAbsolute = Path.Combine(storageProfile.Absolute, fileName);
            var profileUrl = InitializeUrl(storageProfile.Relative, fileName, null);

            // Save the file first.
            File.WriteAllBytes(profileAbsolute, initializer.Profile.Buffer);

            #endregion

            // Account initialization.
            var person = new Person();
            var doctor = new Doctor();

            person.FirstName = initializer.FirstName;
            person.LastName = initializer.LastName;
            person.FullName = person.FirstName + " " + person.LastName;
            person.Birthday = initializer.Birthday;
            person.Gender = (byte) initializer.Gender;
            person.Email = initializer.Email;
            person.Password = initializer.Password;
            person.Phone = initializer.Phone;
            person.Address = initializer.Address;
            person.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            person.Role = (byte) Role.Doctor;
            person.Status = (byte) StatusAccount.Pending;
            doctor.SpecialtyId = specialty.Id;
            doctor.PlaceId = place.Id;
            doctor.ProfileUrl = profileUrl;
            doctor.ProfilePhysicPath = profileAbsolute;

            // Assign personal information to patient.
            person.Doctor = doctor;

            try
            {
                // Save patient data to database.
                await _repositoryAccountExtended.InitializePersonAsync(person);

                // Tell doctor to wait for admin confirmation.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Edit doctor profile.
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> EditDoctorAsync([FromBody] EditDoctorProfileViewModel editor)
        {
            // Filter hasn't been initialized. Initialize it and do validation.
            if (editor == null)
            {
                editor = new EditDoctorProfileViewModel();
                Validate(editor);
            }

            // Invalid model.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Address is defined.
            if (!string.IsNullOrWhiteSpace(editor.Address))
                requester.Address = editor.Address;

            // Phone is defined.
            if (!string.IsNullOrWhiteSpace(editor.Phone))
                requester.Phone = editor.Phone;

            // Password is defined.
            if (!string.IsNullOrWhiteSpace(editor.Password))
                requester.Password = editor.Password;

            // First name is defined.
            if (!string.IsNullOrWhiteSpace(editor.FirstName))
                requester.FirstName = editor.FirstName;

            // Last name is defined.
            if (!string.IsNullOrWhiteSpace(editor.LastName))
                requester.LastName = editor.LastName;

            // Gender is defined.
            if (editor.Gender != null)
                requester.Gender = (byte) editor.Gender;

            // Birthday is defined.
            if (editor.Birthday != null)
                requester.Birthday = editor.Birthday;

            if (requester.Doctor == null)
                requester.Doctor = new Doctor();

            // Update person full name.
            requester.FullName = $"{requester.FirstName} {requester.LastName}";

            // Update the last modified.
            requester.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Place is defined.
            if (editor.Place != null)
                requester.Doctor.PlaceId = editor.Place.Value;

            try
            {
                // Save account.
                requester = await _repositoryAccountExtended.EditPersonProfileAsync(requester.Id, requester);

                // Respond information to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Doctor = new
                    {
                        requester.Id,
                        requester.Email,
                        requester.Password,
                        requester.FirstName,
                        requester.LastName,
                        requester.Birthday,
                        requester.Phone,
                        requester.Gender,
                        requester.Role,
                        requester.Created,
                        requester.LastModified,
                        requester.Status,
                        requester.Address,
                        Photo = requester.PhotoUrl
                    }
                });
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Filter and respond a list of doctors to client.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/doctor/filter")]
        [HttpPost]
        public async Task<HttpResponseMessage> FilterDoctorAsync([FromBody] FilterDoctorViewModel filter)
        {
            #region Request parameters validation

            // Filter hasn't been initialized. Initialize it and do validation.
            if (filter == null)
            {
                filter = new FilterDoctorViewModel();
                Validate(filter);
            }

            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result filter & handling

            try
            {
                filter.Status = StatusAccount.Active;
                var result = await _repositoryAccountExtended.FilterDoctorsAsync(filter);

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
                        Photo = x.Person.PhotoUrl,
                        x.Rank,
                        Specialty = new
                        {
                            x.Specialty.Id,
                            x.Specialty.Name
                        },
                        Place = new
                        {
                            x.Place.Id,
                            x.Place.City,
                            x.Place.Country
                        },
                        x.Voters,
                        x.Person.Created,
                        x.Person.LastModified
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Repository of specialty.
        /// </summary>
        private readonly IRepositorySpecialty _repositorySpecialty;

        /// <summary>
        ///     Repository of places.
        /// </summary>
        private readonly IRepositoryPlace _repositoryPlace;

        /// <summary>
        ///     Repository of storage.
        /// </summary>
        private readonly IRepositoryStorage _repositoryStorage;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which provides function to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        #endregion
    }
}