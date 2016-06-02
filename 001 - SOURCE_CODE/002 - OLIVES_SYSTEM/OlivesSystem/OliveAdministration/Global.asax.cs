using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using DotnetSignalR.Controllers;
using DotnetSignalR.Models;
using DotnetSignalR.Repository;
using Neo4jClient;
using Newtonsoft.Json;
using Shared.Interfaces;

namespace DotnetSignalR
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #region IoC Initialization

            var builder = new ContainerBuilder();

            // You can register controllers all at once using assembly scanning...
            //builder.RegisterControllers(typeof(AdminController).Assembly);

            //// ...or you can register individual controlllers manually.
            builder.RegisterType<AdminController>().InstancePerRequest();
            builder.RegisterType<DoctorController>().InstancePerRequest();

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
            var graphClient = new GraphClient(new Uri(dbSetting.Url), dbSetting.Username, dbSetting.Password);
            graphClient.Connect();

            #endregion

            #region IoC registration

            // Repository account registration.
            var repositoryAccount = new RepositoryAccount(graphClient);
            builder.RegisterType<RepositoryAccount>()
                .As<IRepositoryAccount>()
                .OnActivating(e => e.ReplaceInstance(repositoryAccount))
                .SingleInstance();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            #endregion
        }
    }
}