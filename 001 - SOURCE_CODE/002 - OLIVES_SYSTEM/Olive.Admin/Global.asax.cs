using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using log4net.Config;
using OliveAdmin.Attributes;
using OliveAdmin.Interfaces;
using OliveAdmin.Module;
using OliveAdmin.Repositories;
using Shared.Interfaces;
using Shared.Repositories;
using Shared.Services;

namespace OliveAdmin
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            #region Route configuration

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #endregion

            #region IoC registration

            var containerBuilder = new ContainerBuilder();

            //// ...or you can register individual controlllers manually.
            containerBuilder.RegisterControllers(typeof(WebApiApplication).Assembly);

            #region Repositories

            containerBuilder.RegisterType<OliveDataContext>().As<IOliveDataContext>().SingleInstance();
            containerBuilder.RegisterType<RepositoryAccountExtended>().As<IRepositoryAccountExtended>().SingleInstance();
            containerBuilder.RegisterType<RepositoryPlace>().As<IRepositoryPlace>().SingleInstance();
            containerBuilder.RegisterType<RepositoryMedicalCategory>().As<IRepositoryMedicalCategory>().SingleInstance();
            containerBuilder.RegisterType<TimeService>().As<ITimeService>().SingleInstance();

            #endregion

            #region Properties

            // Olives authorization validate attribute
            containerBuilder.RegisterType<MvcAuthorizeAttribute>().PropertiesAutowired();
            containerBuilder.RegisterFilterProvider();
            #endregion

            #region Modules

            // Log4net module registration (this is for logging)
            XmlConfigurator.Configure();
            containerBuilder.RegisterModule<Log4NetModule>();

            #endregion

            var container = containerBuilder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            #endregion
        }
    }
}