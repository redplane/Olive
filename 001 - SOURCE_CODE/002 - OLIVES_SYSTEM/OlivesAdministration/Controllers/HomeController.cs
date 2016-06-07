using System.Web.Http;

namespace OlivesAdministration.Controllers
{
    public class HomeController : ApiController
    {
        [HttpPut]
        public void Put()
        {
            var a = 1;
        }

        [HttpDelete]
        public void Delete()
        {
            var a = 1;
        }
    }
}