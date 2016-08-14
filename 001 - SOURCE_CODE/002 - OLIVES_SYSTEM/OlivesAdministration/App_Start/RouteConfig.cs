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
            //routes.MapRoute("AdminSignin", "signin", new {controller = "Home", action = "Index"});
            //routes.MapRoute("AdminMain", "admin", new {controller = "Home", action = "Index"});
            //routes.MapRoute("AdminRequest", "admin/requests/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            //routes.MapRoute("AdminPatients", "admin/patients/{id}/{*queryvalues}",
            //    new {controller = "Home", action = "Index", id = UrlParameter.Optional});
            //routes.MapRoute("AdminDoctors", "admin/doctors/{id};{page}",
            //    new {controller = "Home", action = "Index", id = UrlParameter.Optional});
            //routes.MapRoute("AdminPlaces", "admin/places/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            //routes.MapRoute("AdminCategories", "admin/categories/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                "Default",
                "{*url}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}