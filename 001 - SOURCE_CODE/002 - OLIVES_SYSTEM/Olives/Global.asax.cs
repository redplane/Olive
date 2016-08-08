using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Olives
{
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        ///     This function is called when web api application is start for the first time.
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}