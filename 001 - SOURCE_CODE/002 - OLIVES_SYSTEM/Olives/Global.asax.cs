﻿using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
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
            //builder.RegisterControllers(typeof(AdminController).Assembly);

            //// ...or you can register individual controlllers manually.
            //builder.RegisterType<AdminController>().InstancePerRequest();
            
            builder.RegisterType<AccountController>().InstancePerRequest();
            builder.RegisterType<AccountVerifyController>().InstancePerRequest();
            builder.RegisterType<SpecialtyController>().InstancePerRequest();
            builder.RegisterType<AllergyController>().InstancePerRequest();
            builder.RegisterType<HeartbeatController>().InstancePerRequest();
            builder.RegisterType<SugarbloodController>().InstancePerRequest();


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
             *  Email
             */
            // Stmp setting is invalid
            if (applicationSetting.SmtpSetting == null || !applicationSetting.SmtpSetting.IsValid())
                throw new NotImplementedException("Email configuration hasn't been configured.");

            #endregion

            #region IoC registration

            #region Repository & Services

            // Repository account registration.
            builder.RegisterType<RepositoryAccount>()
                .As<IRepositoryAccount>()
                .SingleInstance();

            // Repository specialty registration.
            builder.RegisterType<RepositorySpecialty>()
                .As<IRepositorySpecialty>()
                .SingleInstance();

            // Repository allergy registration.
            builder.RegisterType<RepositoryAllergy>()
                .As<IRepositoryAllergy>()
                .SingleInstance();

            //  Repository activation code registration.
            builder.RegisterType<RepositoryActivationCode>()
                .As<IRepositoryActivationCode>()
                .SingleInstance();

            // Repository of place registration.
            builder.RegisterType<RepositoryPlace>()
                .As<IRepositoryPlace>()
                .SingleInstance();

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
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            
            #endregion

            XmlConfigurator.Configure();
        }
    }
}