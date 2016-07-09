using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Models;
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

        #region Doctor

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
                Doctors = results.Users.Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Birthday,
                    x.Email,
                    x.Gender,
                    x.Address,
                    Photo = InitializeUrl(_applicationSetting.AvatarStorage.Relative, x.Photo, Values.StandardImageExtension),
                    x.Phone,
                    x.Rank,
                    Specialty = new
                    {
                        x.Specialty.Id,
                        x.Specialty.Name
                    },
                    City = new
                    {
                        x.City.Id,
                        x.City.Name,
                        Country = new
                        {
                            x.City.Country.Id,
                            x.City.Country.Name
                        }
                    }
                }),
                results.Total
            });
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

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
            person.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
            person.Role = (byte) Role.Patient;
            person.Status = (byte) StatusAccount.Pending;

            var patient = new Patient();
            patient.Height = info.Height;
            patient.Weight = info.Weight;
            patient.Money = 0;

            // Assign personal information to patient.
            patient.Person = person;

            try
            {
                // Save patient data to database.
                patient = await _repositoryAccount.InitializePatientAsync(patient);

                // Initialize an activation code.
                var activationCode =
                    await _repositoryActivationCode.InitializeActivationCodeAsync(patient.Person.Id, DateTime.Now);

                // Url construction.
                var url = Url.Link("Default",
                    new {controller = "AccountVerify", action = "Index", code = activationCode.Code});

                // Send the activation code email.
                await
                    _emailService.SendActivationCode(person.Email, Language.OliveActivationCodeEmailTitle,
                        person.FirstName, person.LastName, activationCode, url);

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
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/account/doctor")]
        [HttpPost]
        public async Task<HttpResponseMessage> RegisterDoctor([FromBody] InitializeDoctorViewModel info)
        {
            #region Information validation

            #region ModelState validation

            // Information hasn't been initialize.
            if (info == null)
            {
                // Initialize the default instance and do the validation.
                info = new InitializeDoctorViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            #region Specialty validation

            // Find the list of speciaties match with searched condition.
            var specialty = await _repositorySpecialty.FindSpecialtyAsync(info.Specialty);

            // Invalid results set.
            if (specialty == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new
                    {
                        Errors = new[] {string.Format(Language.ValueIsInvalid, "Specialty")}
                    });
            }
            
            #endregion

            #region City validation

            // Find the list of city match with condition.
            var cities = await _repositoryPlace.FindCityAsync(info.City);
            if (cities == null || cities.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new
                    {
                        Errors = new[] {string.Format(Language.ValueIsInvalid, "City")}
                    });
            }

            // Retrieve the first queried result.
            var city = cities.FirstOrDefault();
            if (city == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new
                    {
                        Errors = new[] {string.Format(Language.ValueIsInvalid, "City")}
                    });
            }

            #endregion

            #region Person validation

            // Check whether email has been used or not.
            var result = await _repositoryAccount.FindPersonAsync(null, info.Email, null, null, null);

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

            #endregion

            // Account initialization.
            var person = new Person();
            person.FirstName = info.FirstName;
            person.LastName = info.LastName;
            person.FullName = person.FirstName + " " + person.LastName;
            person.Birthday = info.Birthday;
            person.Gender = (byte) info.Gender;
            person.Email = info.Email;
            person.Password = info.Password;
            person.Phone = info.Phone;
            person.Address = info.Address;
            person.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
            person.Role = (byte) Role.Doctor;
            person.Status = (byte) StatusAccount.Pending;

            var doctor = new Doctor();
            doctor.SpecialtyId = specialty.Id;
            doctor.SpecialtyName = specialty.Name;
            doctor.CityId = city.Id;

            // Assign personal information to patient.
            doctor.Person = person;

            try
            {
                // Save patient data to database.
                doctor = await _repositoryAccount.InitializeDoctorAsync(doctor);

                // Tell doctor to wait for admin confirmation.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Doctor = new
                    {
                        doctor.Id,
                        doctor.Person.FirstName,
                        doctor.Person.LastName,
                        doctor.Person.Email,
                        doctor.Person.Birthday,
                        doctor.Person.Gender,
                        doctor.Person.Address,
                        doctor.Person.Phone,
                        doctor.Person.Role,
                        doctor.Person.Status,
                        doctor.Person.Photo,
                        doctor.Money,
                        Specialty = new
                        {
                            specialty.Id,
                            specialty.Name
                        },
                        City = new
                        {
                            city.Id,
                            city.Name
                        },
                        Country = new
                        {
                            city.Country.Id,
                            city.Country.Name
                        },
                        doctor.Rank,
                        doctor.Voters,
                        doctor.Person.Created
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

            // File validation.
            using (var memoryStream = new MemoryStream(info.Avatar.Buffer))
            {
                try
                {
                    // Move the seeker pointer to the beginning of stream.
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Read image from stream.
                    var image = Image.FromStream(memoryStream);

                    // Invalid image size.
                    if (image.Height != Values.MaxImageSize && image.Width != Values.MaxImageSize)
                    {
                        // Tell client that he/she is forbidden to change his/her due to the uploaded image.
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            Error = $"{Language.WarnImageIncorrectFormat}"
                        });
                    }

                    // As the requester has existed image before, use that name, otherwise generate a new one.
                    if (!string.IsNullOrEmpty(requester.Photo))
                    {
                        // Update the image full path.
                        var fullPath = Path.Combine(_applicationSetting.AvatarStorage.Absolute, $"{requester.Photo}.{Values.StandardImageExtension}");
                        image.Save(fullPath, ImageFormat.Png);

                        _log.Info($"{requester.Email} has updated avatar successfuly.");
                    }
                    else
                    {
                        // Generate name for image and save image first.
                        var imageName = Guid.NewGuid().ToString("N");
                        var fullPath = Path.Combine(_applicationSetting.AvatarStorage.Absolute, $"{imageName}.{Values.StandardImageExtension}");
                        image.Save(fullPath);
                        _log.Info($"{requester.Email} has uploaded avatar successfuly.");

                        // Update to database.
                        requester.Photo = imageName;

                        // Update information to database.
                        await _repositoryAccount.InitializePersonAsync(requester);
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
                            Photo = InitializeUrl(_applicationSetting.AvatarStorage.Relative, requester.Photo, Values.StandardImageExtension)
                        }
                    });
                }
                catch (BadImageFormatException badImageFormatException)
                {
                    // Log error to file.
                    _log.Error(badImageFormatException.Message, badImageFormatException);

                    // Tell the client the image is incorrect format.
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnImageIncorrectFormat}"
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
                    Photo = InitializeUrl(_applicationSetting.AvatarStorage.Relative, result.Photo, Values.StandardImageExtension)
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
            var activationCode = await _repositoryActivationCode.InitializeActivationCodeAsync(account.Id, DateTime.Now);

            // Url construction.
            var url = Url.Link("Default",
                new {controller = "AccountVerify", action = "Index", code = activationCode.Code});

            // Write an email to user to notify him/her to activate account.
            await
                _emailService.SendActivationCode(info.Email, Language.OliveActivationCodeEmailTitle, account.FirstName,
                    account.LastName, activationCode, url);

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