﻿using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Constants;
using Olives.Enumerations;
using Olives.Interfaces;
using Olives.ViewModels;
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
    [Route("api/patient")]
    public class PatientController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="repositoryCode"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="repositoryStorage"></param>
        /// <param name="timeService"></param>
        /// <param name="emailService"></param>
        /// <param name="log"></param>
        public PatientController(
            IRepositoryAccountExtended repositoryAccountExtended, IRepositoryToken repositoryCode,
            IRepositoryRelationship repositoryRelation,
            IRepositoryStorage repositoryStorage,
            ITimeService timeService, IEmailService emailService,
            ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositoryCode = repositoryCode;
            _repositoryRelation = repositoryRelation;
            _repositoryStorage = repositoryStorage;
            _log = log;
            _timeService = timeService;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a related patient by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FindPatientAsync([FromUri] int id)
        {
            try
            {
                #region Result find

                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Find the patient by using account id.
                var account = await _repositoryAccountExtended.FindPersonAsync(id, null, null, (byte) Role.Patient,
                    StatusAccount.Active);

                // No patient has been found as active in system.
                if (account == null)
                {
                    _log.Error($"There is no patient [Id: {id}] is found as active in system");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                #endregion

                #region Relationship validate

                var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(requester.Id, account.Id);
                if (!isRelationshipAvailable)
                {
                    _log.Error(
                        $"There is no relationship between requester [Id: {requester.Id}] and owner [Id: {account.Id}]");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                #endregion
                
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Patient = new
                    {
                        account.Id,
                        account.Email,
                        account.FirstName,
                        account.LastName,
                        account.Birthday,
                        account.Phone,
                        account.Gender,
                        account.Role,
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
        }

        /// <summary>
        ///     Sign up as a patient asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> InitializePatientAsync([FromBody] InitializePatientViewModel info)
        {
            #region Request parameters 

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

            #endregion

            #region Account initialization & handling

            var person = new Person();
            person.FirstName = info.FirstName;
            person.LastName = info.LastName;
            person.FullName = info.FirstName + " " + info.LastName;
            person.Birthday = info.Birthday;
            person.Gender = (byte) info.Gender;
            person.Email = info.Email;
            person.Password = _repositoryAccountExtended.FindMd5Password(info.Password);
            person.Phone = info.Phone;
            person.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            person.Role = (byte) Role.Patient;
            person.Status = (byte) StatusAccount.Pending;

            // Find avatar storage.
            var storageAvatar = _repositoryStorage.FindStorage("Avatar");

            string avatar = null;
            if (person.Gender == (byte) Gender.Male)
                avatar = ConfigurationManager.AppSettings["AvatarFemale"];
            else
                avatar = ConfigurationManager.AppSettings["AvatarMale"];

            person.PhotoUrl = InitializeUrl(storageAvatar.Relative, avatar, null);
            person.PhotoPhysicPath = Path.Combine(storageAvatar.Absolute, avatar);

            // Assign personal information to patient.
            var patient = new Patient();
            person.Patient = patient;

            try
            {
                // Save patient data to database.
                person = await _repositoryAccountExtended.InitializePersonAsync(person);

                // Initialize activation code and send to client.
                await InitializeActivationCodeAsync(person);

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
                        patient.Person.Created
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
        ///     Find a patient by using specific id.
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> EditPatientAsync([FromBody] EditPatientProfileViewModel editor)
        {
            #region Request parameters validation

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

            #endregion

            #region Information construction

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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
                requester.Password = _repositoryAccountExtended.FindMd5Password(editor.Password);

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

            if (requester.Patient == null)
                requester.Patient = new Patient();

            // Weight is defined.
            if (editor.Weight != null)
                requester.Patient.Weight = editor.Weight;

            // Height is defined.
            if (editor.Height != null)
                requester.Patient.Height = editor.Height;

            #endregion

            #region Result handling

            try
            {
                // Update the last modified.
                requester.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Update the patient.
                requester = await _repositoryAccountExtended.EditPersonProfileAsync(requester.Id, requester);
                
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
                        Photo = requester.PhotoUrl,
                        requester.Patient.Height,
                        requester.Patient.Weight
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
        ///     Filter a list of another patient.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/patient/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> FilterPatientAsync([FromBody] FilterPatientViewModel filter)
        {
            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterPatientViewModel();
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
            filter.Requester = requester.Id;

            // Call the filter function.
            var result = await _repositoryAccountExtended.FilterPatientsAsync(filter);
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Patients = result.Patients.Select(x => new
                {
                    x.Id,
                    x.Person.Email,
                    x.Person.FirstName,
                    x.Person.LastName,
                    x.Person.Birthday,
                    x.Person.Phone,
                    Photo = x.Person.PhotoUrl,
                    x.Person.Address,
                    x.Height,
                    x.Weight
                }),
                result.Total
            });
        }

        /// <summary>
        ///     Request an activation code for not activated account.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/patient/code")]
        [HttpGet]
        public async Task<HttpResponseMessage> RequestActivationCode(
            [FromUri] RequestActivationCodeViewModel initializer)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (initializer == null)
            {
                initializer = new RequestActivationCodeViewModel();
                Validate(initializer);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Account query

            // Find the account whose email and password match with given conditions.
            var account =
                await
                    _repositoryAccountExtended.FindPersonAsync(null, initializer.Email, null, (byte) Role.Patient,
                        StatusAccount.Pending);

            if (account == null)
            {
                _log.Error($"Couldn't find account: '{initializer.Email}'");
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            #endregion

            #region Activation token initialization & handling

            try
            {
                // Initialize activation and send email to client.
                await InitializeActivationCodeAsync(account);

                // Respond status 200 with no content to notify user to check email for activation code.
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
        ///     This function is for generating activation code and send to client.
        /// </summary>
        /// <param name="account"></param>
        private async Task InitializeActivationCodeAsync(Person account)
        {
            #region Token initialization

            var accountToken = new AccountToken();
            accountToken.Code = Guid.NewGuid().ToString();
            accountToken.Expired = DateTime.UtcNow.AddHours(Values.ActivationCodeHourDuration);
            accountToken.Owner = account.Id;
            accountToken.Type = (byte) TypeAccountCode.Activation;

            accountToken = await _repositoryCode.InitializeAccountTokenAsync(accountToken);

            #endregion
            
            // Url construction.
            var url = Url.Link("Olives",
                new {controller = "Service", action = "Verify", code = accountToken.Code});

            // Data which will be bound to email.
            var data = new
            {
                firstName = account.FirstName,
                lastName = account.LastName,
                url,
                expired = accountToken.Expired
            };

            // Write an email to user to notify him/her to activate account.
            await
                _emailService.InitializeEmail(new[] {account.Email}, OlivesValues.TemplateEmailActivationCode, data);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Repository which provides functions to relationship database.
        /// </summary>
        private readonly IRepositoryRelationship _repositoryRelation;

        /// <summary>
        /// Repository which provides functions to storage settings.
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

        /// <summary>
        ///     Repository which provides functions to access account code database.
        /// </summary>
        private readonly IRepositoryToken _repositoryCode;

        /// <summary>
        ///     Service which provides functions to access mail sending service.
        /// </summary>
        private readonly IEmailService _emailService;

        #endregion
    }
}