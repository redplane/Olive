using System.Web.Mvc;

namespace OlivesAdministration.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("signin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}