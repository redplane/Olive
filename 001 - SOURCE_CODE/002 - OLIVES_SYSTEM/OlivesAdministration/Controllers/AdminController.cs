using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OlivesAdministration.Models;
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
        #region Properties

        /// <summary>
        ///     Repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        /// Application setting.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="applicationSetting"></param>
        public AdminController(IRepositoryAccount repositoryAccount, ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _applicationSetting = applicationSetting;
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
            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            
            // Pass parameter to login function. 
            var admin =
                await
                    _repositoryAccount.FindPersonAsync(null, loginViewModel.Email, loginViewModel.Password,
                        (byte) Role.Admin, StatusAccount.Active);

            // If no result return, that means no account.
            if (admin == null)
            {
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
                    Photo = InitializeUrl(_applicationSetting.AvatarStorage.Relative, admin.Photo, Values.StandardImageExtension),
                    admin.LastModified
                }
            });
        }

        #endregion
    }
}