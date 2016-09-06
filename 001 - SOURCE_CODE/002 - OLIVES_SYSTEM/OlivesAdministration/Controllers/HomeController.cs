using System.Web.Mvc;
using Olive.Admin.Attributes;
using Shared.Enumerations;

namespace Olive.Admin.Controllers
{
    [MvcAuthorize(new [] { Role.Admin, })]
    public class HomeController : Controller
    {
        /// <summary>
        /// This function is for rendering home page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}