﻿using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
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

namespace Olives
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
            //builder.RegisterControllers(typeof(AccountController).Assembly).InstancePerRequest();

            //// ...or you can register individual controlllers manually.
            //builder.RegisterType<AdminController>().InstancePerRequest();
            builder.RegisterApiControllers(typeof (WebApiApplication).Assembly);
            builder.RegisterControllers(typeof (WebApiApplication).Assembly);
            
            #endregion

            #region General application configuration

            // Initialize an instance of application setting.
            var applicationSetting = LoadApplicationSetting();

            #endregion

            #region IoC registration

            #region Repository

            // Repository account registration.
            builder.RegisterType<RepositoryAccount>()
                .As<IRepositoryAccount>()
                .SingleInstance();

            // Repository specialty registration.
            builder.RegisterType<RepositorySpecialty>()
                .As<IRepositorySpecialty>()
                .SingleInstance();

            // Repository specialty registration.
            builder.RegisterType<RepositoryHeartbeat>()
                .As<IRepositoryHeartbeat>()
                .SingleInstance();

            // Repository heartbeat registration.
            builder.RegisterType<RepositoryAllergy>()
                .As<IRepositoryAllergy>()
                .SingleInstance();

            //  Repository activation code registration.
            builder.RegisterType<RepositoryCode>()
                .As<IRepositoryActivationCode>()
                .SingleInstance();

            // Repository of place registration.
            builder.RegisterType<RepositoryPlace>()
                .As<IRepositoryPlace>()
                .SingleInstance();

            // Repository of addiction registration.
            builder.RegisterType<RepositoryAddiction>()
                .As<IRepositoryAddiction>()
                .SingleInstance();

            // Repository of sugarblood registration.
            builder.RegisterType<RepositorySugarblood>()
                .As<IRepositorySugarblood>()
                .SingleInstance();

            builder.RegisterType<RepositoryAppointment>()
                .As<IRepositoryAppointment>()
                .SingleInstance();

            builder.RegisterType<RepositoryMedical>()
                .As<IRepositoryMedical>()
                .SingleInstance();

            builder.RegisterType<RepositoryRating>()
                .As<IRepositoryRating>()
                .SingleInstance();

            #endregion

            #region Services

            // Email service.
            var emailService = new EmailService(applicationSetting.SmtpSetting);

            // Load email templates.
            if (applicationSetting.SmtpSetting.EmailTemplates != null &&
                applicationSetting.SmtpSetting.EmailTemplates.Length > 0)
            {
                foreach (var email in applicationSetting.SmtpSetting.EmailTemplates)
                {
                    var path = Server.MapPath($"~/{email.Path}");
                    emailService.LoadEmailTemplate(email.Name, path, email.Core);
                }
            }

            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .OnActivating(e => e.ReplaceInstance(emailService))
                .SingleInstance();

            // File service.
            builder.RegisterType<FileService>()
                .As<IFileService>()
                .SingleInstance();
            
            #endregion

            #region Attributes

            // OlivesAuthorize attribute registration (to access dependency)
            builder.RegisterType<OlivesAuthorize>().PropertiesAutowired();
            builder.RegisterType<MedicalCategoryValidateAttribute>().PropertiesAutowired();
            builder.RegisterType<PlaceValidateAttribute>().PropertiesAutowired();
            builder.RegisterType<ImageMaxSizeValidateAttribute>().PropertiesAutowired();

            #endregion

            #region Modules

            // Log4net module registration (this is for logging)
            builder.RegisterModule<Log4NetModule>();

            // Application setting instance.
            builder.RegisterInstance(applicationSetting).As<ApplicationSetting>();

            #endregion

            // Web api dependency registration.
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            #endregion
        }

        /// <summary>
        /// This function is for loading application setting.
        /// </summary>
        /// <returns></returns>
        private ApplicationSetting LoadApplicationSetting()
        {
            // Initialize an instance of application setting.
            var applicationSetting = new ApplicationSetting();

            // Retrieve file name which stores database configuration.
            var applicationConfig = ConfigurationManager.AppSettings["ApplicationConfigFile"];

            // Find the file on physical path.
            var applicationConfigFile = Server.MapPath($"~/{applicationConfig}.json");

            // Invalid application configuration file.
            if (!File.Exists(applicationConfigFile))
                throw new NotImplementedException($"{applicationConfigFile} is required to make server run properly.");

            var info = File.ReadAllText(applicationConfigFile);
            applicationSetting = JsonConvert.DeserializeObject<ApplicationSetting>(info);

            #region Avatar storage folder

            var fullPublicStoragePath = Server.MapPath(applicationSetting.AvatarStorage.Relative);
            if (!Directory.Exists(fullPublicStoragePath))
                Directory.CreateDirectory(fullPublicStoragePath);

            // Update application public storage.
            applicationSetting.AvatarStorage.Absolute = fullPublicStoragePath;

            #endregion

            #region Medical image storage folder

            // Invalid private storage folder.
            var fullMedicalImageStoragePath = Server.MapPath(applicationSetting.MedicalImageStorage.Relative);
            if (!Directory.Exists(fullMedicalImageStoragePath))
                Directory.CreateDirectory(fullMedicalImageStoragePath);

            // Update application private storage folder.
            applicationSetting.MedicalImageStorage.Absolute = fullMedicalImageStoragePath;

            #endregion

            #region Prescription storage folder

            // Invalid private storage folder.
            var fullPrescriptionImagePath = Server.MapPath(applicationSetting.PrescriptionStorage.Relative);
            if (!Directory.Exists(fullPrescriptionImagePath))
                Directory.CreateDirectory(fullPrescriptionImagePath);

            // Update application private storage folder.
            applicationSetting.PrescriptionStorage.Absolute = fullPrescriptionImagePath;

            #endregion

            // Stmp setting is invalid
            if (applicationSetting.SmtpSetting == null || !applicationSetting.SmtpSetting.IsValid())
                throw new NotImplementedException("Email configuration hasn't been configured.");

            return applicationSetting;
        }
    }
}