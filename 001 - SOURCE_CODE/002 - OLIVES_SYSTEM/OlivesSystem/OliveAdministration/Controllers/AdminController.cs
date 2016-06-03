using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Shared.Interfaces;
using Shared.ViewModels;

namespace DotnetSignalR.Controllers
{
    public class AdminController : ParentController
    {
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public AdminController(IRepositoryAccount repositoryAccount) : base(repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        ///     This function is for posting login information to service.
        ///     HttpStatusCode != 200 means login is not successful.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            // Response initialization.
            var response = new ResponseViewModel();

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                response.Errors = RetrieveValidationErrors(ModelState);
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(response);
            }

            // Pass parameter to login function. 
            var result = await _repositoryAccount.LoginAsync(loginViewModel);
            
            // If no result return, that means no account.
            if (result == null)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(null);
            }

            Response.StatusCode = (int) HttpStatusCode.OK;
            response.Data = result;

            return Json(response);
        }
    }
}