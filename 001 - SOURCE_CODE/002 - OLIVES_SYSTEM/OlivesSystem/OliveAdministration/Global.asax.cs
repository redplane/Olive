using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using DotnetSignalR.Controllers;
using DotnetSignalR.Interfaces;
using DotnetSignalR.Repository;

namespace DotnetSignalR
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            var builder = new ContainerBuilder();

            // You can register controllers all at once using assembly scanning...
            //builder.RegisterControllers(typeof(HomeController).Assembly);

            // ...or you can register individual controlllers manually.
            builder.RegisterType<HomeController>().InstancePerRequest();

            var names = new[] {"11", "22", "33", "44"};
            builder.RegisterType<RepositoryProduct>()
                .As<IRepositoryProduct>()
                .OnActivating(e => e.ReplaceInstance(new RepositoryProduct(names)))
                .SingleInstance();
            
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}