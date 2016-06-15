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
using Neo4jClient.Transactions;
using Newtonsoft.Json;
using OlivesAdministration.Attributes;
using OlivesAdministration.Controllers;
using Shared.Constants;
using Shared.Enumerations;
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

        private async void InitializeDemoData(ITransactionalGraphClient graphClient)
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
            admin.Role = AccountRole.Admin;
            admin.Gender = AccountGender.Male;

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
            admin.Password = "buiquoctrong199x";
            admin.Created = DateTime.Now.Ticks;
            admin.Birthday = DateTime.Now.Ticks;
            admin.Id = "adeb011a01a44db08b09dcf0bf2fcd5c";
            admin.Role = AccountRole.Admin;
            admin.Gender = AccountGender.Male;

            graphClient.Cypher
                .Merge($"(n:Person {{ Id: '{admin.Id}'}})")
                .OnCreate()
                .Set("n = {info}")
                .WithParam("info", admin)
                .ExecuteWithoutResults();

            #endregion

            #region Doctors creation

            // TODO: Remove this on production
            for (var i = 0; i < 50; i++)
            {
                var doctor = new Doctor();
                doctor.Id = Guid.NewGuid().ToString("N");
                doctor.LastName = $"LastName{i}";
                doctor.FirstName = $"FirstName{i}";
                doctor.Birthday = DateTime.Now.Ticks;
                doctor.Created = DateTime.Now.Ticks;
                doctor.Email = $"doctor{i}@gmail.com";
                doctor.Created = DateTime.Now.Ticks;
                doctor.Password = $"doctorpassword{i}";
                doctor.Phone = $"00000{i}";
                doctor.Gender = (i % 2 == 0) ? AccountGender.Male : AccountGender.Female;
                doctor.Role = AccountRole.Doctor;
                doctor.Photo = string.Format("https://imageshack.com?photo={0}", i);
                doctor.Rank = i;
                doctor.Speciality = $"Speciality[{i}]";
                doctor.Voters = i;
                doctor.Status = i > 25 ? AccountStatus.Inactive : AccountStatus.Active;

                await graphClient.Cypher
                    .Merge($"(n:Person {{ Id: '{doctor.Id}'}})")
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
                patient.Gender = AccountGender.Male;
                patient.Email = $"linhdse031{i}{(i%2 == 0 ? "@fpt.edu.vn" : "@yahoo.com.vn")}";
                patient.Password = $"PatientPassword";
                patient.Phone = $"01234567{i}";
                patient.Money = i;
                patient.Created = DateTime.Now.Ticks;
                patient.Latitude = i;
                patient.Longitude = i;
                patient.Role = AccountRole.Patient;
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