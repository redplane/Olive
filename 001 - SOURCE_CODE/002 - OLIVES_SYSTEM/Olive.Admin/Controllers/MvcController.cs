using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OliveAdmin.Controllers
{
    public class MvcController : Controller
    {
        /// <summary>
        /// This function is for retrieving error messages from ModelState object.
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns></returns>
        protected IEnumerable<string> FindValidationError(ModelStateDictionary modelStateDictionary)
        {
            return modelStateDictionary.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage);
        }
    }
}