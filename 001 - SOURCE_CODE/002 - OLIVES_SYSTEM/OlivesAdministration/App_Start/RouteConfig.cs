using System.Web.Mvc;
using System.Web.Routing;

namespace OlivesAdministration
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // main admin URLs
            routes.MapRoute("AdminSignin", "signin", new {controller = "Home", action = "Index"});
            routes.MapRoute("AdminMain", "admin", new {controller = "Home", action = "Index"});
            routes.MapRoute("AdminPatients", "admin/patients/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional});
            routes.MapRoute("AdminDoctors", "admin/doctors/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional});

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}