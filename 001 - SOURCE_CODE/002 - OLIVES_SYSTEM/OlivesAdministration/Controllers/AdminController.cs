using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Models;
using Shared.Constants;
using Shared.Enumerations;
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
        /// <param name="applicationSetting"></param>
        /// <param name="log"></param>
        public AdminController(IRepositoryAccountExtended repositoryAccountExtended,
            ApplicationSetting applicationSetting, ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _applicationSetting = applicationSetting;
            _log = log;
        }

        #endregion

        #region Login

        /// <summary>
        ///     This function is for posting login information to service.
        ///     HttpStatusCode != 200 means login is not successful.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [Route("api/admin/login")]
        [HttpPost]
        public async Task<HttpResponseMessage> Login([FromBody] LoginViewModel loginViewModel)
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
                        Photo =
                            InitializeUrl(_applicationSetting.AvatarStorage.Relative, admin.Photo,
                                Values.StandardImageExtension),
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

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account.
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Application setting.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

        /// <summary>
        ///     Instance for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}