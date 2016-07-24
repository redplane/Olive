using System.Web.Http;
using MultipartFormDataMediaFormatter;

namespace Olives
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Make web API support multipart/form-data request.
            config.Formatters.Add(new MultipartFormDataFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );


            config.Routes.MapHttpRoute("ApiRequireAction", "api/{controller}/{action}/{id}",
                new {id = RouteParameter.Optional}
                );
        }
    }
}