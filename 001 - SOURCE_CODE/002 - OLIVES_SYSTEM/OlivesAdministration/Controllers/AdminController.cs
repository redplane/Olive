using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Resources;
using Shared.ViewModels;

namespace OlivesAdministration.Controllers
{
    public class AdminController : ParentController
    {
        #region Properties

        /// <summary>
        ///     Repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public AdminController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        #endregion

        #region Login

        /// <summary>
        ///     This function is for posting login information to service.
        ///     HttpStatusCode != 200 means login is not successful.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Login([FromBody] LoginViewModel loginViewModel)
        {
            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            // Update role Admin to login view model.
            loginViewModel.Role = Roles.Admin;

            // Pass parameter to login function. 
            var result = await _repositoryAccount.LoginAsync(loginViewModel);

            // If no result return, that means no account.
            if (result == null)
            {
                ModelState.AddModelError("Credential", Language.InvalidLoginInfo);
                return Request.CreateResponse(HttpStatusCode.NotFound, RetrieveValidationErrors(ModelState));
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { User = result});
        }

        #endregion
    }
}