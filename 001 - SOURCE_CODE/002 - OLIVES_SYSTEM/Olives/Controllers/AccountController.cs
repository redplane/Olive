using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Constants;
using Olives.Interfaces;
using Olives.Models;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Initialize;

namespace Olives.Controllers
{
    public class AccountController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryActivationCode"></param>
        /// <param name="repositorySpecialty"></param>
        /// <param name="repositoryPlace"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        /// <param name="applicationSetting"></param>
        public AccountController(IRepositoryAccount repositoryAccount,
            IRepositoryActivationCode repositoryActivationCode, IRepositorySpecialty repositorySpecialty,
            IRepositoryPlace repositoryPlace, ILog log, IEmailService emailService,
            ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryActivationCode = repositoryActivationCode;
            _repositorySpecialty = repositorySpecialty;
            _repositoryPlace = repositoryPlace;
            _log = log;
            _emailService = emailService;
            _applicationSetting = applicationSetting;
        }

        #endregion

        #region Patient

        /// <summary>
        ///     Find a patient by using specific id.
        /// </summary>
        /// <returns></returns>
        [Route("api/patient/profile")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> EditPatientProfile([FromBody] EditPatientProfileViewModel editor)
        {
            // Model hasn't been initialized.
            if (editor == null)
            {
                editor = new EditPatientProfileViewModel();
                Validate(editor);
            }

            // ModelState is invalid.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Information construction

            // First name is defined.
            if (!string.IsNullOrWhiteSpace(editor.FirstName))
                requester.FirstName = editor.FirstName;

            // Last name is defined.
            if (!string.IsNullOrWhiteSpace(editor.LastName))
                requester.LastName = editor.LastName;

            // Birthday is defined.
            if (editor.Birthday != null)
                requester.Birthday = editor.Birthday;

            // Password is defined.
            if (!string.IsNullOrWhiteSpace(editor.Password))
                requester.Password = editor.Password;

            // Gender is defined.
            if (editor.Gender != null)
                requester.Gender = (byte) editor.Gender;

            // Phone is defined.
            if (!string.IsNullOrWhiteSpace(editor.Phone))
                requester.Phone = editor.Phone;

            // Address is defined.
            if (!string.IsNullOrWhiteSpace(editor.Address))
                requester.Address = editor.Address;

            // Update person full name.
            requester.FullName = requester.FirstName + " " + requester.LastName;

            var patient = new Patient();
            patient = requester.Patient;

            // Weight is defined.
            if (editor.Weight != null)
                patient.Weight = editor.Weight;

            // Height is defined.
            if (editor.Height != null)
                patient.Height = editor.Height;

            requester.Patient = patient;

            #endregion

            // Update the last modified.
            requester.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

            // Update the patient.
            requester = await _repositoryAccount.EditPersonProfileAsync(requester.Id, requester);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = new
                {
                    patient.Person.Id,
                    patient.Person.Email,
                    patient.Person.Password,
                    patient.Person.FirstName,
                    patient.Person.LastName,
                    patient.Person.Birthday,
                    patient.Person.Phone,
                    patient.Person.Gender,
                    patient.Person.Role,
                    patient.Person.Created,
                    patient.Person.LastModified,
                    patient.Person.Status,
                    patient.Person.Address,
                    Photo =
                        InitializeUrl(_applicationSetting.AvatarStorage.Relative, patient.Person.Photo,
                            Values.StandardImageExtension)
                }
            });
        }

        /// <summary>
        ///     Filter a list of another patient.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/people/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> FilterAnotherPeople([FromBody] FilterAnotherPatientViewModel filter)
        {
            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterAnotherPatientViewModel();
                Validate(filter);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Filter initialization.
            var filterPatient = new FilterPatientViewModel();
            filterPatient.Email = filter.Email;
            filterPatient.Phone = filter.Phone;
            filterPatient.Name = filter.Name;
            filterPatient.MinBirthday = filter.MinBirthday;
            filterPatient.MaxBirthday = filter.MaxBirthday;
            filterPatient.Gender = filter.Gender;

            // Call the filter function.
            var result = await _repositoryAccount.FilterPatientAsync(filterPatient, requester);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Users = result.Patients.Select(x => new
                {
                    x.Id,
                    x.Person.Email,
                    x.Person.FirstName,
                    x.Person.LastName,
                    x.Person.Birthday,
                    x.Person.Phone,
                    Photo =
                        InitializeUrl(_applicationSetting.AvatarStorage.Relative, x.Person.Photo,
                            Values.StandardImageExtension),
                    x.Person.Address
                }),
                result.Total
            });
        }

        #endregion

        #region Doctor

        /// <summary>
        ///     Find a doctor by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/doctor")]
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> FindDoctor([FromUri] int id)
        {
            // Find the doctor by using id.
            var doctor = await _repositoryAccount.FindDoctorAsync(id, StatusAccount.Active);

            // Doctor is not found.
            if (doctor == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });


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
                    Photo =
                        InitializeUrl(_applicationSetting.AvatarStorage.Relative, doctor.Person.Photo,
                            Values.StandardImageExtension),
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
        ///     Filter and respond a list of doctors to client.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/doctor/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> FilterDoctor([FromBody] FilterDoctorViewModel filter)
        {
            // Filter hasn't been initialized. Initialize it and do validation.
            if (filter == null)
            {
                filter = new FilterDoctorViewModel();
                Validate(filter);
            }

            // Prevent patient from searching sensitive information
            filter.MinMoney = null;
            filter.MaxMoney = null;
            filter.MinCreated = null;
            filter.MaxCreated = null;
            filter.Status = (int) StatusAccount.Active;
            filter.MinLastModified = null;
            filter.MaxLastModified = null;
            filter.MinMoney = null;
            filter.MaxMoney = null;

            // Invalid model.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            var results = await _repositoryAccount.FilterDoctorAsync(filter);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Doctors = results.Doctors.Select(x => new
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
                    Photo =
                        InitializeUrl(_applicationSetting.AvatarStorage.Relative, x.Person.Photo,
                            Values.StandardImageExtension),
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
                results.Total
            });
        }

        /// <summary>
        ///     Edit doctor profile.
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        [Route("api/doctor/profile")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> EditDoctorProfile([FromBody] EditDoctorProfileViewModel editor)
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

            // Update person full name.
            requester.FullName = $"{requester.FirstName} {requester.LastName}";

            // Update the last modified.
            requester.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

            // Place is defined.
            if (editor.Place != null)
            {
                // Find the place by using id.
                var place = await _repositoryPlace.FindPlaceAsync(editor.Place, null, null, null, null);
                requester.Doctor.PlaceId = place.Id;
                requester.Doctor.City = place.City;
                requester.Doctor.Country = place.Country;
            }

            try
            {
                // Save account.
                requester = await _repositoryAccount.EditPersonProfileAsync(requester.Id, requester);

                // Respond information to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    User = new
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
                        Photo =
                            InitializeUrl(_applicationSetting.AvatarStorage.Relative, requester.Photo,
                                Values.StandardImageExtension)
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

        #endregion

        #region Sign up

        /// <summary>
        ///     Sign up as a patient asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/account/patient")]
        [HttpPost]
        public async Task<HttpResponseMessage> RegisterPatient([FromBody] InitializePatientViewModel info)
        {
            // Information hasn't been initialize.
            if (info == null)
            {
                // Initialize the default instance and do the validation.
                info = new InitializePatientViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Check whether email has been used or not.
            var result = await _repositoryAccount.FindPersonAsync(null, info.Email, null, null, null);

            // Found a patient. This means email has been used before.
            if (result != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new
                    {
                        Error = $"{Language.WarnAccountAlreadyExists}"
                    });
            }

            // Account initialization.
            var person = new Person();
            person.FirstName = info.FirstName;
            person.LastName = info.LastName;
            person.FullName = info.FirstName + " " + info.LastName;
            person.Birthday = info.Birthday;
            person.Gender = (byte) info.Gender;
            person.Email = info.Email;
            person.Password = info.Password;
            person.Phone = info.Phone;
            person.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
            person.Role = (byte) Role.Patient;
            person.Status = (byte) StatusAccount.Pending;

            var patient = new Patient();
            patient.Height = info.Height;
            patient.Weight = info.Weight;
            patient.Money = 0;

            // Assign personal information to patient.
            person.Patient = patient;

            try
            {
                // Save patient data to database.
                person = await _repositoryAccount.InitializePersonAsync(person);

                // Initialize an activation code.
                var activationCode =
                    await
                        _repositoryActivationCode.InitializeAccountCodeAsync(patient.Person.Id,
                            TypeAccountCode.Activation, DateTime.UtcNow);

                // Url construction.
                var url = Url.Link("Default",
                    new {controller = "Service", action = "Verify", code = activationCode.Code});

                // Send the activation code email.
                await
                    _emailService.InitializeTokenEmail(person.Email, Language.OliveActivationCodeEmailTitle,
                        person.FirstName, person.LastName, activationCode, url, EmailType.Activation);

                // Tell doctor to wait for admin confirmation.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Patient = new
                    {
                        patient.Id,
                        patient.Person.FirstName,
                        patient.Person.LastName,
                        patient.Person.Email,
                        patient.Person.Birthday,
                        patient.Person.Gender,
                        patient.Person.Address,
                        patient.Person.Phone,
                        patient.Person.Role,
                        patient.Person.Status,
                        patient.Person.Photo,
                        patient.Money,
                        patient.Weight,
                        patient.Height,
                        patient.Person.Created
                    }
                });
            }
            catch (Exception exception)
            {
                // There is something wrong with server.
                // Log the error.
                _log.Error($"Cannot create account: '{person.Email}'", exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Sign up as a patient asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/account/doctor")]
        [HttpPost]
        public async Task<HttpResponseMessage> RegisterDoctor([FromBody] InitializeDoctorViewModel initializer)
        {
            #region Information validation

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

            // Check whether email has been used or not.
            var result = await _repositoryAccount.FindPersonAsync(null, initializer.Email, null, null, null);

            // Found a person. This means email has been used before.
            if (result != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new
                    {
                        Error = $"{Language.WarnAccountAlreadyExists}"
                    });
            }

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
            person.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
            person.Role = (byte) Role.Doctor;
            person.Status = (byte) StatusAccount.Pending;

            // Update specialty information.
            doctor.SpecialtyId = specialty.Id;
            doctor.SpecialtyName = specialty.Name;

            // Update place information.
            doctor.PlaceId = place.Id;
            doctor.City = place.City;
            doctor.Country = place.Country;

            // Assign personal information to patient.
            person.Doctor = doctor;

            try
            {
                // Save patient data to database.
                person = await _repositoryAccount.InitializePersonAsync(person);

                // Tell doctor to wait for admin confirmation.
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
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                // There is something wrong with server.
                // Log the error.
                _log.Error($"Cannot create account: '{person.Email}'", exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     This function is for posting image as account avatar.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/account/avatar")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> PostAvatar([FromBody] InitializeAvatarViewModel info)
        {
            // Model validation.
            if (info == null)
            {
                info = new InitializeAvatarViewModel();
                Validate(info);
            }

            if (!ModelState.IsValid)
            {
                _log.Error("Model validation is not successful");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // As the requester has existed image before, use that name, otherwise generate a new one.
                if (!string.IsNullOrEmpty(requester.Photo))
                {
                    // Update the image full path.
                    var fullPath = Path.Combine(_applicationSetting.AvatarStorage.Absolute,
                        $"{requester.Photo}.{Values.StandardImageExtension}");

                    // Save the image to physical disk.
                    info.Avatar.Save(fullPath, ImageFormat.Png);

                    _log.Info($"{requester.Email} has updated avatar successfuly.");
                }
                else
                {
                    // Generate name for image and save image first.
                    var imageName = Guid.NewGuid().ToString("N");

                    // Take the full path.
                    var fullPath = Path.Combine(_applicationSetting.AvatarStorage.Absolute,
                        $"{imageName}.{Values.StandardImageExtension}");

                    // Save the avatar file to disk.
                    info.Avatar.Save(fullPath);

                    // Log the information.
                    _log.Info($"{requester.Email} has uploaded avatar successfuly.");

                    // Update to database.
                    requester.Photo = imageName;

                    // Update information to database.
                    await _repositoryAccount.InitializePersonAsync(requester);

                    // Log the information.
                    _log.Info($"{requester.Email} has saved avatar successfully");
                }


                // Everything is successful. Tell client the result.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    User = new
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
                        Photo =
                            InitializeUrl(_applicationSetting.AvatarStorage.Relative, requester.Photo,
                                Values.StandardImageExtension)
                    }
                });
            }
            catch (Exception exception)
            {
                // Log error to file.
                _log.Error(exception.Message, exception);

                // Tell the client the server has some error occured, please try again.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        #endregion

        #region Forgot password

        /// <summary>
        ///     Request an email contains forgot password token asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/account/forgot")]
        [HttpGet]
        public async Task<HttpResponseMessage> FindLostPassword([FromUri] ForgotPasswordViewModel info)
        {
            #region Request parameters validation

            // Information hasn't been initialize.
            if (info == null)
            {
                // Initialize the default instance and do the validation.
                info = new ForgotPasswordViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Check whether email has been used or not.
            var result = await _repositoryAccount.FindPersonAsync(null, info.Email, null, null, StatusAccount.Active);

            // Found a patient. This means email has been used before.
            if (result == null)
            {
                // Tell the client that person is not found.
                _log.Error($"Person [Email: {info.Email}] is not found as active in system");

                // Tell the client about this error.
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new
                    {
                        Error = $"{Language.WarnAccountInvalid}"
                    });
            }

            try
            {
                // Initialize an activation code.
                var findPasswordToken =
                    await
                        _repositoryActivationCode.InitializeAccountCodeAsync(result.Id, TypeAccountCode.ForgotPassword,
                            DateTime.UtcNow);

                // Url construction.
                var url = Url.Link("Default",
                    new {controller = "Service", action = "FindPassword"});

                // Send the activation code email.
                await
                    _emailService.InitializeTokenEmail(info.Email, Language.OliveForgotPasswordEmailTitle,
                        result.FirstName, result.LastName, findPasswordToken, url, EmailType.FindPassword);

                // Tell doctor to wait for admin confirmation.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // There is something wrong with server.
                // Log the error.
                _log.Error($"Cannot create account: '{result.Email}'", exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Request an email contains forgot password token asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/account/forgot")]
        [HttpPost]
        public async Task<HttpResponseMessage> SubmitLostPassword([FromBody] InitializeNewPasswordViewModel initializer)
        {
            #region Request parameters validation

            // Information hasn't been initialize.
            if (initializer == null)
            {
                // Initialize the default instance and do the validation.
                initializer = new InitializeNewPasswordViewModel();
                Validate(initializer);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Writing log.
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Token validation

            // Check whether email has been used or not.
            var token =
                await
                    _repositoryActivationCode.FindAccountCodeAsync(null, (byte) TypeAccountCode.ForgotPassword,
                        initializer.Token);

            // Token couldn't be found.
            if (token == null)
            {
                _log.Error($"Token [Code: {initializer.Token}] is not found in the system.");
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
            }

            // Token is expired.
            if (DateTime.UtcNow > token.Expired)
            {
                // Log the error.
                _log.Error(
                    $"Token [Code: {token.Code}] is expired. It should be activated before {token.Expired.ToString("F")}");

                // Tell the client about the error.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnTokenExpired}"
                });
            }

            #endregion

            #region Information modify

            try
            {
                // Update client new password.
                await _repositoryActivationCode.InitializeNewAccountPassword(token, initializer.Password);

                // Tell doctor to wait for admin confirmation.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // There is something wrong with server.
                // Log the error.
                _log.Error($"Cannot create account: '{token.Person.Email}'", exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Login

        /// <summary>
        ///     This function is for authenticate an user account.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [Route("api/account/login")]
        [HttpPost]
        public async Task<HttpResponseMessage> Login([FromBody] LoginViewModel loginViewModel)
        {
            if (loginViewModel == null)
            {
                loginViewModel = new LoginViewModel();
                Validate(loginViewModel);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid login request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Pass parameter to login function. 
            var results = await _repositoryAccount.LoginAsync(loginViewModel);

            // If no result return, that means no account.
            if (results == null || results.Count != 1)
            {
                _log.Error(
                    $"No record has been found with {loginViewModel.Email} - {loginViewModel.Password} - {loginViewModel.Role} ");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first result.
            var result = results.FirstOrDefault();
            if (result == null)
            {
                _log.Error(
                    $"No record has been found with {loginViewModel.Email} - {loginViewModel.Password} - {loginViewModel.Role} ");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requested user is not a patient or a doctor.
            if (result.Role != (byte) Role.Patient && result.Role != (byte) Role.Doctor)
            {
                _log.Error($"{loginViewModel.Email} is a admin, therefore, it cannot be used here.");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Login is failed because of account is pending.
            if ((StatusAccount) result.Status == StatusAccount.Pending)
            {
                // Tell doctor to contact admin for account verification.
                if (result.Role == (byte) Role.Doctor)
                {
                    _log.Error($"Access is forbidden because {loginViewModel.Email} is waiting for admin confirmation");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnPendingAccount}"
                    });
                }

                _log.Error($"Access is forbidden because {loginViewModel.Email} is waiting for admin confirmation");
                // Tell patient to access his/her email to verify the account.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnPendingAccount}"
                });
            }

            // Login is failed because of account has been disabled.
            if ((StatusAccount) result.Status == StatusAccount.Inactive)
            {
                _log.Error($"Access is forbidden because {loginViewModel.Email} has been disabled");
                // Tell patient to access his/her email to verify the account.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnDisabledAccount}"
                });
            }
            _log.Info($"{loginViewModel.Email} has logged in successfully");
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = new
                {
                    result.Id,
                    result.Email,
                    result.Password,
                    result.FirstName,
                    result.LastName,
                    result.Birthday,
                    result.Phone,
                    result.Gender,
                    result.Role,
                    result.Created,
                    result.LastModified,
                    result.Status,
                    result.Address,
                    Photo =
                        InitializeUrl(_applicationSetting.AvatarStorage.Relative, result.Photo,
                            Values.StandardImageExtension)
                }
            });
        }

        /// <summary>
        ///     Request an activation code for not activated account.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/account/code")]
        [HttpGet]
        public async Task<HttpResponseMessage> RequestActivationCode([FromUri] AccountViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid account information");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] {Language.InvalidRequestParameters}
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid account information");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Account query

            // Find the account whose email and password match with given conditions.
            var account =
                await _repositoryAccount.FindPersonAsync(null, info.Email, info.Password, (byte) Role.Patient, null);
            if (account == null)
            {
                _log.Error($"Couldn't find account: '{info.Email}' : '{info.Password}'");
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            // Account is not waiting for being activated. Treat this as not found.
            if (account.Status != (byte) StatusAccount.Pending)
            {
                _log.Error($"Couldn't create activation code for '{info.Email}' due to its status {account.Status}");
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            #endregion

            // Initialize activation code.
            var activationToken =
                await
                    _repositoryActivationCode.InitializeAccountCodeAsync(account.Id, TypeAccountCode.Activation,
                        DateTime.UtcNow);

            // Url construction.
            var url = Url.Link("Default",
                new {controller = "Service", action = "Verify", code = activationToken.Code});

            // Write an email to user to notify him/her to activate account.
            await
                _emailService.InitializeTokenEmail(info.Email, Language.OliveActivationCodeEmailTitle, account.FirstName,
                    account.LastName, activationToken, url, EmailType.Activation);

            // Respond status 200 with no content to notify user to check email for activation code.
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of activation codes.
        /// </summary>
        private readonly IRepositoryActivationCode _repositoryActivationCode;

        /// <summary>
        ///     Repository of specialty.
        /// </summary>
        private readonly IRepositorySpecialty _repositorySpecialty;

        /// <summary>
        ///     Repository of places.
        /// </summary>
        private readonly IRepositoryPlace _repositoryPlace;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used for sending emails.
        /// </summary>
        private readonly IEmailService _emailService;

        /// <summary>
        ///     Property which contains settings of application which had been deserialized from json file.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

        #endregion
    }
}