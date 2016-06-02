using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Shared.Constants;
using Shared.Interfaces;

namespace DotnetSignalR.Controllers
{
    public class ParentController : Controller
    {
        /// <summary>
        ///     Class which stores accounts.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Initialize an instance of ParentController with given parameters.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public ParentController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        /// <summary>
        ///     Retrieve validation errors and bind 'em to list.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected IEnumerable<string> RetrieveValidationErrors(ModelStateDictionary modelState)
        {
            // Invalid model state.
            if (modelState == null)
                return new List<string>();

            return modelState.Keys.SelectMany(key => modelState[key].Errors.Select(error => error.ErrorMessage));
        }

        /// <summary>
        ///     Check if the request comes from a person with a specific role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        protected async Task<HttpStatusCode> IsInValidRoleAsync(byte role)
        {
            // Retrieve account and password from request.
            var accountEmail = Request.Headers[Constants.RequestHeaderAccountEmail];
            var accountPassword = Request.Headers[Constants.RequestHeaderAccountPassword];

            // Invalid account name or password.
            if (string.IsNullOrEmpty(accountEmail) || string.IsNullOrEmpty(accountPassword))
                return HttpStatusCode.Unauthorized;

            // Retrieve person whose properties match conditions.
            var person = await _repositoryAccount.GetPersonExistAsync(accountEmail, accountPassword, role);
            if (person.Role != role)
                return HttpStatusCode.Forbidden;
            
            return HttpStatusCode.OK;
        }
    }
}