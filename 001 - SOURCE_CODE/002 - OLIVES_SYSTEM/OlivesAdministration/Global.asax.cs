using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using OlivesAdministration.Attributes;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Models;
using OlivesAdministration.Module;
using OlivesAdministration.Repositories;
using Shared.Interfaces;
using Shared.Repositories;

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

            #region General application configuration

            // Initialize an instance of application setting.
            var applicationSetting = new ApplicationSetting();

            // Retrieve file name which stores database configuration.
            var applicationConfig = ConfigurationManager.AppSettings["ApplicationConfigFile"];

            // Find the file on physical path.
            var applicationConfigFile = Server.MapPath($"~/{applicationConfig}.json");

            // Invalid application configuration file.
            if (!File.Exists(applicationConfigFile))
                throw new NotImplementedException($"{applicationConfigFile} is required to make server run properly.");

            var info = File.ReadAllText(applicationConfigFile);
            applicationSetting = JsonConvert.DeserializeObject<ApplicationSetting>(info);

            #endregion

            #region IoC registration

            var builder = new ContainerBuilder();

            //// ...or you can register individual controlllers manually.
            builder.RegisterApiControllers(typeof (WebApiApplication).Assembly);

            #region Repositories

            builder.RegisterType<OliveDataContext>().As<IOliveDataContext>().SingleInstance();
            builder.RegisterType<RepositoryAccountExtended>().As<IRepositoryAccountExtended>().SingleInstance();
            builder.RegisterType<RepositoryPlace>().As<IRepositoryPlace>().SingleInstance();
            builder.RegisterType<RepositoryMedicalCategory>().As<IRepositoryMedicalCategory>().SingleInstance();

            var repositoryStorage = new RepositoryStorage(HttpContext.Current);
            foreach (var key in applicationSetting.Storage.Keys)
                repositoryStorage.InitializeStorage(key, applicationSetting.Storage[key]);
            builder.RegisterType<RepositoryStorage>()
                .As<IRepositoryStorage>()
                .OnActivating(e => e.ReplaceInstance(repositoryStorage)).SingleInstance();

            #endregion

            #region Properties

            // Olives authorization validate attribute
            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();

            #endregion

            #region Modules

            // Log4net module registration (this is for logging)
            builder.RegisterModule<Log4NetModule>();

            // Application setting instance.
            builder.RegisterInstance(applicationSetting).As<ApplicationSetting>();

            #endregion

            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            #endregion
        }
    }
}