using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using log4net.Config;
using Neo4jClient;
using Newtonsoft.Json;
using OlivesAdministration.Attributes;
using OlivesAdministration.Controllers;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models;
using Shared.Models.Nodes;
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

            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            #endregion

            //InitializeDemoData(graphClient);

            XmlConfigurator.Configure();
        }

        private async void InitializeDemoData(GraphClient graphClient)
        {
            #region Admin creation

            // TODO: Remove this code.
            var admin = new Person();
            admin.FirstName = "Nguyen";
            admin.LastName = "Linh";
            admin.Email = "redplane_dt@yahoo.com.vn";
            admin.Password = "redplane";
            admin.Created = DateTime.Now.Ticks;
            admin.Birthday = DateTime.Now.Ticks;
            admin.Id = "2f28d661db9449fdb90d0879f5231fde";
            admin.Password = "redplane_dt@yahoo.com.vn";
            admin.Role = Roles.Admin;
            admin.Gender = Gender.Male;

            graphClient.Cypher
                .Merge($"(n:Person {{ Id: '{admin.Id}'}})")
                .OnCreate()
                .Set("n = {info}")
                .WithParam("info", admin)
                .ExecuteWithoutResults();


            admin = new Person();
            admin.FirstName = "Trong";
            admin.LastName = "Bui";
            admin.Email = "trong.buiquoc@gmail.com";
            admin.Password = "redplane";
            admin.Created = DateTime.Now.Ticks;
            admin.Birthday = DateTime.Now.Ticks;
            admin.Id = "adeb011a01a44db08b09dcf0bf2fcd5c";
            admin.Password = "b453133b7ee466c6dc500ed30b5fd75a";
            admin.Role = Roles.Admin;
            admin.Gender = Gender.Male;

            graphClient.Cypher
                .Merge($"(n:Person {{ Id: '{admin.Id}'}})")
                .OnCreate()
                .Set("n = {info}")
                .WithParam("info", admin)
                .ExecuteWithoutResults();

            #endregion

            #region Doctors creation

            // TODO: Remove this code.
            var doctorIds = new[]
            {
                "a43bd26af38045cfb87765a6460bc3be", "11b9190a803b46109a120630588de37a",
                "b14e626e9e0f40f8824e415c57bdd942", "cdad3953299449d68c48ec3c015b78db"
            };

            for (var i = 0; i < doctorIds.Length; i++)
            {
                var doctor = new Doctor();
                doctor.Id = doctorIds[i];
                doctor.LastName = $"LastName{i}";
                doctor.FirstName = $"FirstName{i}";
                doctor.Birthday = DateTime.Now.Ticks;
                doctor.Created = DateTime.Now.Ticks;
                doctor.Email = $"doctor{i}@gmail.com";
                doctor.Created = DateTime.Now.Ticks;
                doctor.Password = $"doctorpassword{i}";
                doctor.Phone = $"00000{i}";
                doctor.Gender = Gender.Female;
                doctor.Role = Roles.Doctor;
                doctor.Photo = string.Format("https://imageshack.com?photo={0}", i);
                doctor.Rank = i;
                doctor.Speciality = $"Speciality[{i}]";
                doctor.Voters = i;

                await graphClient.Cypher
                    .Merge($"(n:Person {{ Id: '{doctorIds[i]}'}})")
                    .OnCreate()
                    .Set("n = {info}")
                    .Set("n = {info}")
                    .WithParam("info", doctor)
                    .ExecuteWithoutResultsAsync();
            }

            #endregion

            #region Patient creation

            // TODO: Remove this code.
            for (var i = 0; i < 100; i++)
            {
                var patient = new Patient();
                patient.Id = Guid.NewGuid().ToString("N");
                patient.FirstName = $"PatientFirstName[{i}]";
                patient.LastName = $"PatientLastName[{i}]";
                patient.Birthday = DateTime.Now.Ticks;
                patient.Gender = Gender.Male;
                patient.Email = $"linhdse031{i}@fpt.edu.vn";
                patient.Password = $"PatientPassword{i}";
                patient.Phone = $"01234567{i}";
                patient.Money = i;
                patient.Created = DateTime.Now.Ticks;
                patient.Latitude = i;
                patient.Longitude = i;
                patient.Role = Roles.Patient;
                patient.Height = i;
                patient.Weight = i;

                await graphClient.Cypher
                    .Merge($"(n:Person {{ Id: '{patient.Id}'}})")
                    .OnCreate()
                    .Set("n = {info}")
                    .Set("n = {info}")
                    .WithParam("info", patient)
                    .ExecuteWithoutResultsAsync();
            }

            #endregion
        }
    }
}