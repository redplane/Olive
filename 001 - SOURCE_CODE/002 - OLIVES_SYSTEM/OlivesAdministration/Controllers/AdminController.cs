using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OlivesAdministration.Attributes;
using OlivesAdministration.Interfaces;
using OlivesAdministration.ViewModels.Edit;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;

namespace OlivesAdministration.Controllers
{
    public class AdminController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        public AdminController(
            IRepositoryAccountExtended repositoryAccountExtended,
            ILog log,
            ITimeService timeService)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _log = log;
            _timeService = timeService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for posting login information to service.
        ///     HttpStatusCode != 200 means login is not successful.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [Route("api/admin/login")]
        [HttpPost]
        public async Task<HttpResponseMessage> LoginAsync([FromBody] LoginViewModel loginViewModel)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (loginViewModel == null)
            {
                loginViewModel = new LoginViewModel();
                Validate(loginViewModel);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Account find & handling

            try
            {
                // Pass parameter to login function. 
                var admin =
                    await
                        _repositoryAccountExtended.FindPersonAsync(null, loginViewModel.Email, loginViewModel.Password,
                            (byte) Role.Admin, StatusAccount.Active);

                // If no result return, that means no account.
                if (admin == null)
                {
                    _log.Error($"There is no admin [Id: {loginViewModel.Email}] is found in database");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    User = new
                    {
                        admin.Id,
                        admin.LastName,
                        admin.FirstName,
                        admin.Birthday,
                        admin.Gender,
                        admin.Email,
                        admin.Password,
                        admin.Phone,
                        admin.Created,
                        admin.Address,
                        admin.Role,
                        admin.Status,
                        Photo = admin.PhotoUrl,
                        admin.LastModified
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
        [OlivesAuthorize(new[] {Role.Admin})]
        [Route("api/admin/profile")]
        public async Task<HttpResponseMessage> EditAdminProfileAsync([FromBody] EditAdminProfileViewModel editor)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (editor == null)
            {
                editor = new EditAdminProfileViewModel();
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
                        Photo = requester.PhotoUrl
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

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account.
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Instance for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used to access time calculation functions.
        /// </summary>
        private readonly ITimeService _timeService;

        #endregion
    }
}