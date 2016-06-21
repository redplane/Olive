using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using log4net.Config;
using Newtonsoft.Json;
using OlivesAdministration.Attributes;
using OlivesAdministration.Controllers;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Models;
using Shared.Models;
using OlivesAdministration.Repositories;
namespace OlivesAdministration
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            #region Route configuration

            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #endregion

            #region IoC Initialization

            var builder = new ContainerBuilder();

            // You can register controllers all at once using assembly scanning...
            //builder.RegisterControllers(typeof(AdminController).Assembly);

            //// ...or you can register individual controlllers manually.
            builder.RegisterType<AdminController>().InstancePerRequest();
            builder.RegisterType<DoctorController>().InstancePerRequest();
            builder.RegisterType<PatientController>().InstancePerRequest();
            builder.RegisterType<PersonController>().InstancePerRequest();

            #endregion

            #region Database connection

            // Retrieve file name which stores database configuration.
            var olivesDbFile = ConfigurationManager.AppSettings["GraphDbConfigFile"];

            // Find the file on physical path.
            var dbConfigsFile = Server.MapPath($"~/{olivesDbFile}.json");

            // Invalid database configuration file.
            if (!File.Exists(dbConfigsFile))
            {
                var exception = new Exception($"Invalid database file. {dbConfigsFile} doesn't exist.");
                throw exception;
            }

            var fileInfo = File.ReadAllText(dbConfigsFile);
            var dbSetting = JsonConvert.DeserializeObject<DbSetting>(fileInfo);

            // Invalid databse configuration.
            if (dbSetting == null)
                throw new Exception("Invalid database configuration.");

            // Graphdabase client connection construction.
            //var graphClient = new GraphClient(new Uri(dbSetting.Url), dbSetting.Username, dbSetting.Password);
            //graphClient.Connect();

            #endregion

            #region IoC registration

            // Repository account registration.
            builder.RegisterType<RepositoryAccount>()
                .As<IRepositoryAccount>()
                .SingleInstance();

            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            #endregion
            
            XmlConfigurator.Configure();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
    }
}