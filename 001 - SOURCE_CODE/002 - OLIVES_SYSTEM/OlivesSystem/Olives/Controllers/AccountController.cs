using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Shared.Constants;
using Shared.Interfaces;
using Shared.ViewModels;

namespace Olives.Controllers
{
    public class AccountController : ParentController
    {

        #region Properties

        /// <summary>
        /// Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public AccountController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        #endregion

        #region Login

        /// <summary>
        /// This function is for authenticate an user account.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            // Response initialization.
            var response = new ResponseViewModel();

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                response.Errors = RetrieveValidationErrors(ModelState);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(response);
            }

            // Update role Admin to login view model.
            loginViewModel.Role = Roles.Admin;

            // Pass parameter to login function. 
            var result = await _repositoryAccount.LoginAsync(loginViewModel);

            // If no result return, that means no account.
            if (result == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            
            // Requested user is not a patient or a doctor.
            if (result.Role != Roles.Patient && result.Role != Roles.Doctor)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            
            Response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = result;

            return Json(response);
        }

        #endregion
    }
}