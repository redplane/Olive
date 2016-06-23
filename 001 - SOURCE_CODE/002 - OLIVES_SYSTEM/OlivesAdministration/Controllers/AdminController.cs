using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Shared.Enumerations;
using Shared.Interfaces;
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
            loginViewModel.Role = AccountRole.Admin;

            // Pass parameter to login function. 
            var results = await _repositoryAccount.LoginAsync(loginViewModel);

            // If no result return, that means no account.
            if (results == null)
            {
                ModelState.AddModelError("Credential", Language.InvalidLoginInfo);
                return Request.CreateResponse(HttpStatusCode.NotFound, RetrieveValidationErrors(ModelState));
            }

            // Results conflict with each other.
            if (results.Count != 1)
            {
                ModelState.AddModelError("Credential", Language.LoginConflict);
                return Request.CreateResponse(HttpStatusCode.Conflict, RetrieveValidationErrors(ModelState));
            }

            var result = results.Select(x => new
            {
                x.Id,
                x.LastName,
                x.FirstName,
                x.Birthday,
                x.Gender,
                x.Email,
                x.Password,
                x.Phone,
                x.Created,
                x.Latitude,
                x.Longitude,
                x.Address,
                x.Role,
                x.Status,
                x.Photo,
                x.LastModified
            }).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new {User = result});
        }

        #endregion
    }
}