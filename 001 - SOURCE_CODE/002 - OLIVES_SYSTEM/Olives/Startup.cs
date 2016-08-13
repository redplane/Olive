using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Newtonsoft.Json;
using Olives;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Interfaces.Medical;
using Olives.Interfaces.PersonalNote;
using Olives.Models;
using Olives.Models.Emails;
using Olives.Module;
using Olives.Repositories;
using Olives.Repositories.Medical;
using Olives.Repositories.PersonalNote;
using Olives.Services;
using Owin;
using Shared.Interfaces;
using Shared.Repositories;
using Shared.Services;
using AutofacDependencyResolver = Autofac.Integration.Mvc.AutofacDependencyResolver;

[assembly: OwinStartup(typeof (Startup))]

namespace Olives
{
    public class Startup
    {
        /// <summary>
        ///     Configuration function of OWIN Startup.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            #region IoC Initialization

            var builder = new ContainerBuilder();

            // You can register controllers all at once using assembly scanning...
            //builder.RegisterControllers(typeof(AccountController).Assembly).InstancePerRequest();

            //// ...or you can register individual controlllers manually.
            //builder.RegisterType<AdminController>().InstancePerRequest();
            builder.RegisterApiControllers(typeof (Startup).Assembly);
            builder.RegisterControllers(typeof (Startup).Assembly);

            // Register your SignalR hubs.
            builder.RegisterHubs(typeof (Startup).Assembly);

            #endregion

            #region General application configuration

            // Initialize an instance of application setting.
            var applicationSetting = LoadApplicationSetting();

            #endregion

            #region IoC registration

            #region Modules

            // Log4net module registration (this is for logging)
            var log = new Log4NetModule();
            builder.RegisterModule<Log4NetModule>();

            // Application setting instance.
            builder.RegisterInstance(applicationSetting).As<ApplicationSetting>();

            #endregion

            #region Repositories

            builder.RegisterType<OliveDataContext>().As<IOliveDataContext>().SingleInstance();

            // Repository account registration.
            builder.RegisterType<RepositoryAccountExtended>().As<IRepositoryAccountExtended>().SingleInstance();

            builder.RegisterType<RepositoryRelationship>().As<IRepositoryRelationship>().SingleInstance();
            builder.RegisterType<RepositoryRelationshipRequest>().As<IRepositoryRelationshipRequest>().SingleInstance();

            builder.RegisterType<RepositorySpecialty>().As<IRepositorySpecialty>().SingleInstance();

            builder.RegisterType<RepositoryHeartbeat>().As<IRepositoryHeartbeat>().SingleInstance();
            builder.RegisterType<RepositoryAllergy>().As<IRepositoryAllergy>().SingleInstance();
            builder.RegisterType<RepositoryAddiction>().As<IRepositoryAddiction>().SingleInstance();
            builder.RegisterType<RepositoryBloodSugar>().As<IRepositoryBloodSugar>().SingleInstance();
            builder.RegisterType<RepositoryBloodPressure>().As<IRepositoryBloodPressure>().SingleInstance();
            builder.RegisterType<RepositoryDiary>().As<IRepositoryDiary>().SingleInstance();

            builder.RegisterType<RepositoryCode>().As<IRepositoryCode>().SingleInstance();
            builder.RegisterType<RepositoryPlace>().As<IRepositoryPlace>().SingleInstance();
            builder.RegisterType<RepositoryAppointment>().As<IRepositoryAppointment>().SingleInstance();
            builder.RegisterType<RepositoryRating>().As<IRepositoryRating>().SingleInstance();
            builder.RegisterType<RepositoryNotification>().As<IRepositoryNotification>().SingleInstance();
            builder.RegisterType<RepositoryMessage>().As<IRepositoryMessage>().SingleInstance();
            builder.RegisterType<RepositoryRealTimeConnection>().As<IRepositoryRealTimeConnection>().SingleInstance();

            var repositoryStorage = new RepositoryStorage(HttpContext.Current);
            foreach (var key in applicationSetting.Storage.Keys)
                repositoryStorage.InitializeStorage(key, applicationSetting.Storage[key]);
            builder.RegisterType<RepositoryStorage>()
                .As<IRepositoryStorage>()
                .OnActivating(e => e.ReplaceInstance(repositoryStorage)).SingleInstance();

            #region Medical repositories

            builder.RegisterType<RepositoryExperimentNote>().As<IRepositoryExperimentNote>().SingleInstance();
            builder.RegisterType<RepositoryMedicalCategory>().As<IRepositoryMedicalCategory>().SingleInstance();
            builder.RegisterType<RepositoryMedicalImage>().As<IRepositoryMedicalImage>().SingleInstance();
            builder.RegisterType<RepositoryMedicalNote>().As<IRepositoryMedicalNote>().SingleInstance();
            builder.RegisterType<RepositoryMedicalRecord>().As<IRepositoryMedicalRecord>().SingleInstance();
            builder.RegisterType<RepositoryPrescription>().As<IRepositoryPrescription>().SingleInstance();
            builder.RegisterType<RepositoryPrescriptionImage>().As<IRepositoryPrescriptionImage>().SingleInstance();

            #endregion

            #endregion

            #region Services

            var emailService = LoadEmailService(applicationSetting.SmtpSetting);
            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .OnActivating(e => e.ReplaceInstance(emailService))
                .SingleInstance();
            builder.RegisterType<FileService>().As<IFileService>().SingleInstance();
            builder.RegisterType<TimeService>().As<ITimeService>().SingleInstance();
            builder.RegisterType<NotificationService>().As<INotificationService>().SingleInstance();

            #endregion

            #region Attributes

            // OlivesAuthorize attribute registration (to access dependency)
            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();
            builder.RegisterType<HubAuthorizeAttribute>().PropertiesAutowired();

            builder.RegisterType<MedicalCategoryValidateAttribute>().PropertiesAutowired();
            builder.RegisterType<PlaceValidateAttribute>().PropertiesAutowired();

            #endregion

            // Web api dependency registration.
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            // Container build.
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);

            // When application starts up. Remove all real time connection created before.
            var repositoryRealtimeConnection = DependencyResolver.Current.GetService<IRepositoryRealTimeConnection>();
            repositoryRealtimeConnection.DeleteRealTimeConnectionInfoAsync(null, null, null).Wait();

            // Map all signalr hubs.
            app.MapSignalR();

            #endregion
        }

        /// <summary>
        ///     This function is for loading application setting.
        /// </summary>
        /// <returns></returns>
        private ApplicationSetting LoadApplicationSetting()
        {
            // Initialize an instance of application setting.
            var applicationSetting = new ApplicationSetting();

            // Retrieve file name which stores database configuration.
            var applicationConfig = ConfigurationManager.AppSettings["ApplicationConfigFile"];

            // Find the file on physical path.
            var applicationConfigFile = HttpContext.Current.Server.MapPath($"~/{applicationConfig}.json");

            // Invalid application configuration file.
            if (!File.Exists(applicationConfigFile))
                throw new NotImplementedException($"{applicationConfigFile} is required to make server run properly.");

            var info = File.ReadAllText(applicationConfigFile);
            applicationSetting = JsonConvert.DeserializeObject<ApplicationSetting>(info);

            // Stmp setting is invalid
            if (applicationSetting.SmtpSetting == null || !applicationSetting.SmtpSetting.IsValid())
                throw new NotImplementedException("Email configuration hasn't been configured.");

            return applicationSetting;
        }

        /// <summary>
        ///     Load email settings and bind to email service froom json file.
        /// </summary>
        /// <param name="smtpSetting"></param>
        /// <returns></returns>
        private IEmailService LoadEmailService(SmtpSetting smtpSetting)
        {
            // Retrieve the smtp setting
            var emailSettings = smtpSetting.EmailSettings;

            // No email setting is available in system.
            if (emailSettings == null)
                throw new Exception("No email has been configured in system");

            // Initialize an instance of email service.
            var emailService = new EmailService(smtpSetting);

            foreach (var key in emailSettings.Keys)
            {
                // Retrieve the email setting in the list.
                var emailSetting = emailSettings[key];

                // Email is not configured.
                if (emailSetting == null)
                    throw new Exception($"{key} isn't configured.");

                // Match the relative path to absolute path.
                var absolutePath = HttpContext.Current.Server.MapPath(emailSetting.File);
                var emailModel = new EmailModel(emailSetting.Subject, File.ReadAllText(absolutePath),
                    emailSetting.IsHtml);
                emailService.EmailTemplatesCollection.Add(key, emailModel);
            }

            return emailService;
        }
    }
}