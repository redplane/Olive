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

        
    }
}