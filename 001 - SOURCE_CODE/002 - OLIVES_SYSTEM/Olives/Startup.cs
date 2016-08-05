using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac.Integration.SignalR;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Models;
using Olives.Models.Emails;
using Olives.Module;
using Olives.Repositories;
using Olives.Services;
using Shared.Interfaces;
using Shared.Repositories;
using Owin;
using Shared.Services;

[assembly: OwinStartup(typeof(Olives.Startup))]
namespace Olives
{
    public class Startup
    {
        /// <summary>
        /// Configuration function of OWIN Startup.
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
            builder.RegisterApiControllers(typeof(Startup).Assembly);
            builder.RegisterControllers(typeof(Startup).Assembly);

            // Register your SignalR hubs.
            builder.RegisterHubs(typeof(Startup).Assembly);

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

            // Repository account registration.
            builder.RegisterType<RepositoryAccountExtended>().As<IRepositoryAccountExtended>().SingleInstance();
            builder.RegisterType<RepositoryRelation>().As<IRepositoryRelation>().SingleInstance();
            builder.RegisterType<RepositorySpecialty>().As<IRepositorySpecialty>().SingleInstance();
            builder.RegisterType<RepositoryHeartbeat>().As<IRepositoryHeartbeat>().SingleInstance();
            builder.RegisterType<RepositoryAllergy>().As<IRepositoryAllergy>().SingleInstance();
            builder.RegisterType<RepositoryCode>().As<IRepositoryCode>().SingleInstance();
            builder.RegisterType<RepositoryPlace>().As<IRepositoryPlace>().SingleInstance();
            builder.RegisterType<RepositoryAddiction>().As<IRepositoryAddiction>().SingleInstance();
            builder.RegisterType<RepositoryBloodSugar>().As<IRepositoryBloodSugar>().SingleInstance();
            builder.RegisterType<RepositoryAppointment>().As<IRepositoryAppointment>().SingleInstance();
            builder.RegisterType<RepositoryRating>().As<IRepositoryRating>().SingleInstance();
            builder.RegisterType<RepositoryNotification>().As<IRepositoryNotification>().SingleInstance();
            var repositoryRealtimeConnection = new RepositoryRealTimeConnection();
            builder.RegisterType<RepositoryRealTimeConnection>().As<IRepositoryRealTimeConnection>().OnActivating(e => e.ReplaceInstance(repositoryRealtimeConnection)).SingleInstance();
            builder.RegisterType<RepositoryBloodPressure>().As<IRepositoryBloodPressure>().SingleInstance();
            builder.RegisterType<RepositoryMessage>().As<IRepositoryMessage>().SingleInstance();

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
            builder.RegisterType<EmailService>().As<IEmailService>().OnActivating(e => e.ReplaceInstance(emailService)).SingleInstance();
            builder.RegisterType<FileService>().As<IFileService>().SingleInstance();
            builder.RegisterType<TimeService>().As<ITimeService>();
            builder.RegisterType<NotificationService>().As<INotificationService>().OnActivating(e => e.ReplaceInstance(new NotificationService(repositoryRealtimeConnection)));
            
            #endregion

            #region Attributes

            // OlivesAuthorize attribute registration (to access dependency)
            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();
            builder.RegisterType<HubAuthorizeAttribute>().PropertiesAutowired();

            builder.RegisterType<MedicalCategoryValidateAttribute>().PropertiesAutowired();
            builder.RegisterType<PlaceValidateAttribute>().PropertiesAutowired();
            builder.RegisterType<ImageMaxSizeValidateAttribute>().PropertiesAutowired();

            #endregion

            

            // Web api dependency registration.
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            // Container build.
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(container));
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);

            // When application starts up. Remove all real time connection created before.
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

            #region Avatar storage folder

            var fullPublicStoragePath = HttpContext.Current.Server.MapPath(applicationSetting.AvatarStorage.Relative);
            if (!Directory.Exists(fullPublicStoragePath))
                Directory.CreateDirectory(fullPublicStoragePath);

            // Update application public storage.
            applicationSetting.AvatarStorage.Absolute = fullPublicStoragePath;

            #endregion

            #region Medical image storage folder

            // Invalid private storage folder.
            var fullMedicalImageStoragePath = HttpContext.Current.Server.MapPath(applicationSetting.MedicalImageStorage.Relative);
            if (!Directory.Exists(fullMedicalImageStoragePath))
                Directory.CreateDirectory(fullMedicalImageStoragePath);

            // Update application private storage folder.
            applicationSetting.MedicalImageStorage.Absolute = fullMedicalImageStoragePath;

            #endregion

            #region Prescription storage folder

            // Invalid private storage folder.
            var fullPrescriptionImagePath = HttpContext.Current.Server.MapPath(applicationSetting.PrescriptionImageStorage.Relative);
            if (!Directory.Exists(fullPrescriptionImagePath))
                Directory.CreateDirectory(fullPrescriptionImagePath);

            // Update application private storage folder.
            applicationSetting.PrescriptionImageStorage.Absolute = fullPrescriptionImagePath;

            #endregion

            // Stmp setting is invalid
            if (applicationSetting.SmtpSetting == null || !applicationSetting.SmtpSetting.IsValid())
                throw new NotImplementedException("Email configuration hasn't been configured.");

            return applicationSetting;
        }

        /// <summary>
        /// Load email settings and bind to email service froom json file.
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
                var emailModel = new EmailModel(emailSetting.Subject, File.ReadAllText(absolutePath), emailSetting.IsHtml);
                emailService.EmailTemplatesCollection.Add(key, emailModel);
            }

            return emailService;
        }
    }

}