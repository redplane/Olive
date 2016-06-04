using System;
using System.Collections.Generic;
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
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models.Nodes;

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

            #region Admin creation

            var person = new Person();
            person.FirstName = "Nguyen";
            person.LastName = "Linh";
            person.Email = "redplane_dt@yahoo.com.vn";
            person.Password = "redplane";
            person.Created = DateTime.Now.Ticks;
            person.Birthday = DateTime.Now.Ticks;
            person.Id = "c857a1fe-084c-4926-9ad7-ecfeabd038ff";
            person.Password = "c857a1fe-084c-4926-9ad7-ecfeabd038ff";
            person.Role = Roles.Admin;
            person.Gender = Gender.Male;

            // TODO: Remove this code.

            var parameters = new Dictionary<string, object>();
            parameters.Add("mergeInfo", person);
            parameters.Add("info", person);

            graphClient.Cypher
                .Merge($"(n:Person {{ Id: '{person.Id}'}})")
                .OnCreate()
                .Set("n = {info}")
                .WithParams(parameters)
                .ExecuteWithoutResults();

            var doctorIds = new[]
            {
                "a43bd26a-f380-45cf-b877-65a6460bc3be", "11b9190a-803b-4610-9a12-0630588de37a",
                "b14e626e-9e0f-40f8-824e-415c57bdd942", "cdad3953-2994-49d6-8c48-ec3c015b78db"
            };

            for (var i = 0; i < doctorIds.Length; i++)
            {
                var doctor = new Doctor();
                doctor.Id = doctorIds[i];
                doctor.LastName = $"LastName_{i}";
                doctor.FirstName = $"FirstName_{i}";

                doctor.Created = DateTime.Now.Ticks;
                doctor.Email = $"Email_{i}";
                doctor.Created = DateTime.Now.Ticks;
                doctor.Password = $"Password_{i}";
                doctor.Phone = $"00000{i}";
                doctor.Gender = Gender.Female;
                doctor.Role = Roles.Doctor;

                graphClient.Cypher
                    .Merge($"(n:Person {{ Id: '{doctorIds[i]}'}})")
                    .OnCreate()
                    .Set("n = {info}")
                    .Set("n = {info}")
                    .WithParam("info", doctor)
                    .ExecuteWithoutResults();
            }

            #endregion
        }
    }
}