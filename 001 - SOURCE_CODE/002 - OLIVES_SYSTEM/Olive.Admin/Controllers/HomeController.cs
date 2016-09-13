using System.Web.Mvc;
using OliveAdmin.Attributes;

namespace OliveAdmin.Controllers
{
    [MvcAuthorize]
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