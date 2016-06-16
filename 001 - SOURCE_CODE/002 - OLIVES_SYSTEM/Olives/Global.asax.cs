using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using Autofac;
using Autofac.Integration.WebApi;
using log4net.Config;
using Neo4jClient;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.Controllers;
using Olives.Interfaces;
using Olives.Models;
using Olives.Module;
using Olives.Services;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repositories;
using Olives.ViewModels;

namespace Olives
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            #region Route configuration

            GlobalConfiguration.Configure(WebApiConfig.Register);

            #endregion

            #region IoC Initialization

            var builder = new ContainerBuilder();

            // You can register controllers all at once using assembly scanning...
            //builder.RegisterControllers(typeof(AdminController).Assembly);

            //// ...or you can register individual controlllers manually.
            //builder.RegisterType<AdminController>().InstancePerRequest();
            builder.RegisterType<PatientController>().InstancePerRequest();
            builder.RegisterType<AccountController>().InstancePerRequest();

            #endregion

            #region General application configuration

            // Initialize an instance of application setting.
            var applicationSetting = new ApplicationSetting();

            // Retrieve file name which stores database configuration.
            var applicationConfig = ConfigurationManager.AppSettings["ApplicationConfigFile"];

            // Find the file on physical path.
            var applicationConfigFile = Server.MapPath($"~/{applicationConfig}.json");

            // Invalid database configuration file.
            if (File.Exists(applicationConfigFile))
            {
                var info = File.ReadAllText(applicationConfigFile);
                applicationSetting = JsonConvert.DeserializeObject<ApplicationSetting>(info);
            }

            #endregion

            #region Application settings check

            /* 
             * Graph database 
             */
            // No graph database has been configured.
            if (applicationSetting.Database == null || string.IsNullOrEmpty(applicationSetting.Database.Url))
                throw new NotImplementedException("No graph database has been configured.");

            // Retrieve the database configuration.
            var database = applicationSetting.Database;

            // Graphdabase client connection construction.
            var graphClient = new GraphClient(new Uri(database.Url), database.Username, database.Password);
            graphClient.Connect();

            
            /*
             *  Email
             */
            // Stmp setting is invalid
            //if (applicationSetting.SmtpSetting == null || !applicationSetting.SmtpSetting.IsValid())
            //    throw new NotImplementedException("Email configuration hasn't been configured.");

            #endregion

            #region IoC registration

            #region Repository & Services

            // Repository account registration.
            var repositoryAccount = new RepositoryAccount(graphClient);
            builder.RegisterType<RepositoryAccount>()
                .As<IRepositoryAccount>()
                .OnActivating(e => e.ReplaceInstance(repositoryAccount))
                .SingleInstance();

            // Email service.
             var emailService = new EmailService(applicationSetting.SmtpSetting);

            // Load email templates.
            //if (applicationSetting.SmtpSetting.EmailSettings != null &&
            //    applicationSetting.SmtpSetting.EmailSettings.Length > 0)
            //{
            //    foreach (var emailSetting in applicationSetting.SmtpSetting.EmailSettings)
            //    {
            //        var path = Server.MapPath($"~/{emailSetting.Path}");
            //        emailService.LoadEmailTemplate(emailSetting.Name, path);
            //    }
            //}

            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .OnActivating(e => e.ReplaceInstance(emailService))
                .SingleInstance();

            #endregion

            #region Attributes

            // OlivesAuthorize attribute registration (to access dependency)
            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();

            #endregion

            // Log4net module registration (this is for logging)
            builder.RegisterModule<Log4NetModule>();

            // Web api dependency registration.
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            #endregion

            XmlConfigurator.Configure();
        }
    }
}