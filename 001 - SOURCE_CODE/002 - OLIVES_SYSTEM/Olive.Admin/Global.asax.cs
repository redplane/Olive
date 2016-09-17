using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ArangoDB.Client;
using Autofac;
using Autofac.Integration.Mvc;
using log4net.Config;
using OliveAdmin.Attributes;
using OliveAdmin.Module;
using Shared.Interfaces;
using Shared.Repositories;
using Shared.Services;

namespace OliveAdmin
{
    public class OliveAdmin : HttpApplication
    {
        protected void Application_Start()
        {
            #region Route configuration

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #endregion

            #region IoC registration

            var containerBuilder = new ContainerBuilder();

            //// ...or you can register individual controlllers manually.
            containerBuilder.RegisterControllers(typeof(OliveAdmin).Assembly);

            #region Repositories
            
            var arangoClient = new ArangoDatabase(new DatabaseSharedSetting()
            {
                Url = "http://127.0.0.1:8529/",
                Credential = new NetworkCredential()
                {
                    UserName = "root",
                    Password = "Redplane1"
                },
                Database = "_system"
            });

            containerBuilder.RegisterType<ArangoDatabase>()
                .As<ArangoDatabase>()
                .OnActivating(x => x.ReplaceInstance(arangoClient)).SingleInstance();
            containerBuilder.RegisterType<RepositoryAccount>().As<IRepositoryAccount>().SingleInstance();
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