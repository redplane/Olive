using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Models;
using Olives.Module;
using Olives.Services;
using Shared.Interfaces;
using Shared.Repositories;

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