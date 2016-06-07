﻿using System.Web.Mvc;
using System.Web.Routing;

namespace Olives
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Admin", action = "Login", id = UrlParameter.Optional}
                );
        }
    }
}