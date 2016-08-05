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
using Olives.ViewModels;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    public class AccountController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="repositoryActivationCode"></param>
        /// <param name="repositorySpecialty"></param>
        /// <param name="repositoryPlace"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        /// <param name="timeService"></param>
        /// <param name="applicationSetting"></param>
        public AccountController(
            IRepositoryAccountExtended repositoryAccountExtended, IRepositoryCode repositoryActivationCode,
            ILog log, IEmailService emailService,
            ApplicationSetting applicationSetting)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositoryActivationCode = repositoryActivationCode;
            _log = log;
            _emailService = emailService;
            _applicationSetting = applicationSetting;
        }

        #endregion

        #region Methods
        
        /// <summary>
        ///     This function is for posting image as account avatar.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/account/avatar")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Patient, Role.Doctor })]
        public async Task<HttpResponseMessage> InitializeAvatarAsync([FromBody] InitializeAvatarViewModel info)
        {
            #region Request parameters validation

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

            #endregion

            #region Result initialization & handling

            try
            {

                // Retrieve information of person who sent request.
                var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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
                    await _repositoryAccountExtended.InitializePersonAsync(requester);

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
            #endregion
        }
        
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
            var result =
                await _repositoryAccountExtended.FindPersonAsync(null, info.Email, null, null, StatusAccount.Active);

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
                    new { controller = "Service", action = "FindPassword" });

                // Data which will be bound to email template.
                var data = new
                {
                    firstName = result.FirstName,
                    lastName = result.LastName,
                    url,
                    findPasswordToken.Expired
                };

                // Send the activation code email.
                await
                    _emailService.InitializeEmail(new [] {info.Email}, OlivesValues.TemplateEmailFindPassword, data);

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
                    _repositoryActivationCode.FindAccountCodeAsync(null, (byte)TypeAccountCode.ForgotPassword,
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
        
        /// <summary>
        ///     This function is for authenticate an user account.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [Route("api/account/login")]
        [HttpPost]
        public async Task<HttpResponseMessage> Login([FromBody] LoginViewModel loginViewModel)
        {
            #region Request parameters validation

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

            #endregion
            
            // Pass parameter to login function. 
            var account = await _repositoryAccountExtended.FindPersonAsync(null, loginViewModel.Email, loginViewModel.Password, null, null);
            
            if (account == null)
            {
                _log.Error(
                    $"No record has been found with {loginViewModel.Email} - {loginViewModel.Password} ");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requested user is not a patient or a doctor.
            if (account.Role != (byte)Role.Patient && account.Role != (byte)Role.Doctor)
            {
                _log.Error($"{loginViewModel.Email} is a admin, therefore, it cannot be used here.");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Login is failed because of account is pending.
            if ((StatusAccount)account.Status == StatusAccount.Pending)
            {
                // Tell doctor to contact admin for account verification.
                if (account.Role == (byte)Role.Doctor)
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
            if ((StatusAccount)account.Status == StatusAccount.Inactive)
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
                    account.Id,
                    account.Email,
                    account.Password,
                    account.FirstName,
                    account.LastName,
                    account.Birthday,
                    account.Phone,
                    account.Gender,
                    account.Role,
                    account.Created,
                    account.LastModified,
                    account.Status,
                    account.Address,
                    Photo =
                        InitializeUrl(_applicationSetting.AvatarStorage.Relative, account.Photo,
                            Values.StandardImageExtension)
                }
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Repository of activation codes.
        /// </summary>
        private readonly IRepositoryCode _repositoryActivationCode;
        
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