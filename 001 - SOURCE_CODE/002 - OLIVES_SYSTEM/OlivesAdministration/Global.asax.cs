using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using OlivesAdministration.Attributes;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Module;
using OlivesAdministration.Repositories;
using Shared.Interfaces;
using Shared.Repositories;
using Shared.Services;

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

            #region IoC registration

            var builder = new ContainerBuilder();

            //// ...or you can register individual controlllers manually.
            builder.RegisterApiControllers(typeof (WebApiApplication).Assembly);

            #region Repositories

            builder.RegisterType<OliveDataContext>().As<IOliveDataContext>().SingleInstance();
            builder.RegisterType<RepositoryAccountExtended>().As<IRepositoryAccountExtended>().SingleInstance();
            builder.RegisterType<RepositoryPlace>().As<IRepositoryPlace>().SingleInstance();
            builder.RegisterType<RepositoryMedicalCategory>().As<IRepositoryMedicalCategory>().SingleInstance();
            builder.RegisterType<TimeService>().As<ITimeService>().SingleInstance();

            #endregion

            #region Properties

            // Olives authorization validate attribute
            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();

            #endregion

            #region Modules

            // Log4net module registration (this is for logging)
            log4net.Config.XmlConfigurator.Configure();
            builder.RegisterModule<Log4NetModule>();

            #endregion

            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            #endregion
        }
    }
}