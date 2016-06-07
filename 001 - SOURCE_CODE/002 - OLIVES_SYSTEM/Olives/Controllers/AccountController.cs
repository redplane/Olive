using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Shared.Constants;
using Shared.Interfaces;
using Shared.ViewModels;

namespace Olives.Controllers
{
    public class AccountController : ParentController
    {
        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public AccountController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
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
            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadGateway, RetrieveValidationErrors(ModelState));

            // Update role Admin to login view model.
            loginViewModel.Role = Roles.Admin;

            // Pass parameter to login function. 
            var result = await _repositoryAccount.LoginAsync(loginViewModel);

            // If no result return, that means no account.
            if (result == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // Requested user is not a patient or a doctor.
            if (result.Role != Roles.Patient && result.Role != Roles.Doctor)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        #endregion
    }
}