using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Constants;
using Olives.Enumerations;
using Olives.Interfaces;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;

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
        /// <param name="repositoryStorage"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        /// <param name="fileService"></param>
        public AccountController(
            IRepositoryAccountExtended repositoryAccountExtended, IRepositoryToken repositoryActivationCode,
            IRepositoryStorage repositoryStorage,
            ILog log,
            IEmailService emailService, IFileService fileService)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositoryActivationCode = repositoryActivationCode;
            _repositoryStorage = repositoryStorage;
            _log = log;
            _emailService = emailService;
            _fileService = fileService;
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
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
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
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Retrieve avatar storage setting.
                var storageAvatar = _repositoryStorage.FindStorage(Storage.Avatar);

                // Convert by stream to image.
                var imageAvatar = _fileService.ConvertBytesToImage(info.Avatar.Buffer);

                #region Image save

                // Generate name for image and save image first.
                var imageName = Guid.NewGuid().ToString("N");

                // Take the full path.
                var fullPath = Path.Combine(storageAvatar.Absolute,
                    $"{imageName}.{Values.StandardImageExtension}");

                // Save the avatar file to disk.
                imageAvatar.Save(fullPath, ImageFormat.Png);

                // Log the information.
                _log.Info($"{requester.Email} has uploaded avatar successfuly.");

                #endregion

                #region Image link generation

                // Initialize url and physical path of photo and save to database.
                requester.PhotoUrl = InitializeUrl(storageAvatar.Relative, imageName, Values.StandardImageExtension);
                requester.PhotoPhysicPath = fullPath;

                // Update information to database.
                await _repositoryAccountExtended.InitializePersonAsync(requester);

                // Log the information.
                _log.Info($"{requester.Email} has saved avatar successfully");

                #endregion

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
                        Photo = requester.PhotoUrl
                    }
                });
            }
            catch (Exception exception)
            {
                // Log error to file.
                _log.Error(exception.Message, exception);

                // Tell the client the server has some error occured, please try again.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
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
            var account =
                await _repositoryAccountExtended.FindPersonAsync(null, info.Email, null, null, StatusAccount.Active);

            // Found a patient. This means email has been used before.
            if (account == null)
            {
                _log.Error($"Person [Email: {info.Email}] is not found as active in system");
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new
                    {
                        Error = $"{Language.WarnAccountInvalid}"
                    });
            }

            try
            {
                var accountToken = new AccountToken();
                accountToken.Owner = account.Id;
                accountToken.Code = Guid.NewGuid().ToString();
                accountToken.Type = (byte) TypeAccountCode.ForgotPassword;
                accountToken.Expired = DateTime.UtcNow.AddHours(Values.ActivationCodeHourDuration);

                accountToken = await _repositoryActivationCode.InitializeAccountTokenAsync(accountToken);

                // Url construction.
                var url = Url.Link("Default",
                    new {controller = "Service", action = "FindPassword"});

                // Retrieve the client current time.
                // Data which will be bound to email template.
                var data = new
                {
                    firstName = account.FirstName,
                    lastName = account.LastName,
                    token = accountToken.Code,
                    url,
                    accountToken.Expired,
                };

                // Send the activation code email.
                await
                    _emailService.InitializeEmail(new[] {info.Email}, OlivesValues.TemplateEmailFindPassword, data);

                _log.Error("Create token successful");

                // Tell doctor to wait for admin confirmation.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // There is something wrong with server.
                // Log the error.
                _log.Error($"Cannot create token for account: '{account.Email}'", exception);
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

            #region Token find

            var filter = new FilterAccountTokenViewModel();
            filter.Code = initializer.Token;
            filter.Type = (byte) TypeAccountCode.ForgotPassword;

            // Find the account token.
            var accountToken = await _repositoryActivationCode.FindAccountTokenAsync(filter);

            // Token is not found.
            if (accountToken == null)
            {
                _log.Error($"Token [Code: {initializer.Token}] is not found in the system.");
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
            }

            #endregion

            #region Token validation

            // Token is expired.
            if (DateTime.UtcNow > accountToken.Expired)
            {
                // Log the error.
                _log.Error(
                    $"Token [Code: {accountToken.Code}] is expired. It should be activated before {accountToken.Expired.ToString("F")}");

                // Tell the client about the error.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnTokenExpired}"
                });
            }

            #endregion

            #region Account find

            // Find the account.
            var account =
                await
                    _repositoryAccountExtended.FindPersonAsync(accountToken.Owner, null, null, (byte) Role.Patient,
                        StatusAccount.Pending);

            // No account is not found.
            if (account == null)
            {
                _log.Error($"Account [Id: {accountToken.Owner}] is not found in the system.");
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
            }

            #endregion

            #region Information modify

            try
            {
                // Update account information.
                account.Password = _repositoryAccountExtended.FindMd5Password(initializer.Password);

                // Save account to database.
                await _repositoryAccountExtended.InitializePersonAsync(account);

                try
                {
                    // Delete the token.
                    await _repositoryActivationCode.DeleteAccountTokenAsync(filter);
                }
                catch (Exception exception)
                {
                    _log.Error(exception.Message, exception);
                }

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

            #region Login validation

            // Calculate the hashed password from the original one.
            var hashedPassword = _repositoryAccountExtended.FindMd5Password(loginViewModel.Password);

            // Pass parameter to login function. 
            var account =
                await
                    _repositoryAccountExtended.FindPersonAsync(null, loginViewModel.Email, hashedPassword, null,
                        null);

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
            if ((account.Role != (byte) Role.Patient) && (account.Role != (byte) Role.Doctor))
            {
                _log.Error($"{loginViewModel.Email} is a admin, therefore, it cannot be used here.");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Login is failed because of account is pending.
            if ((StatusAccount) account.Status == StatusAccount.Pending)
            {
                // Tell doctor to contact admin for account verification.
                if (account.Role == (byte) Role.Doctor)
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
            if ((StatusAccount) account.Status == StatusAccount.Inactive)
            {
                _log.Error($"Access is forbidden because {loginViewModel.Email} has been disabled");
                // Tell patient to access his/her email to verify the account.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnDisabledAccount}"
                });
            }

            #endregion
            
            if (account.Role == (byte) Role.Doctor)
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
                        Photo = account.PhotoUrl
                    }
                });

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
                    Photo = account.PhotoUrl,
                    account.Patient.Weight,
                    account.Patient.Height
                }
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository which provides functions to access storage database.
        /// </summary>
        private readonly IRepositoryStorage _repositoryStorage;

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Repository of activation codes.
        /// </summary>
        private readonly IRepositoryToken _repositoryActivationCode;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used for sending emails.
        /// </summary>
        private readonly IEmailService _emailService;

        /// <summary>
        ///     Service which is used for processing file.
        /// </summary>
        private readonly IFileService _fileService;

        #endregion
    }
}