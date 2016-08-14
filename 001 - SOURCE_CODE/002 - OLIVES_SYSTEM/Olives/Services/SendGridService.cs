using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Nustache.Core;
using Olives.Interfaces;
using Olives.Models;
using Olives.Models.Emails;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Olives.Services
{
    public class SendGridService : IEmailService
    {
        #region Properties

        /// <summary>
        ///     Instance saves send grid configuration.
        /// </summary>
        private readonly SendGridAPIClient _sendGridApiClient;

        /// <summary>
        ///     Collection of email templates.
        /// </summary>
        public Dictionary<string, SendGridContent> ContentsCollection { get; set; }

        /// <summary>
        ///     Logging instance which is used for writing log.
        /// </summary>
        public ILog Log { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize an instance of service which is used for broadcasting email.
        /// </summary>
        public SendGridService()
        {
            // Initialize email content collection.
            ContentsCollection = new Dictionary<string, SendGridContent>();
        }

        /// <summary>
        ///     Initialize an instance of service which is used for broadcasting email.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="applicationSetting"></param>
        public SendGridService(HttpContext httpContext, ApplicationSetting applicationSetting) : this()
        {
            if (applicationSetting == null)
                throw new Exception("Application setting is required.");

            // Obtain the configuration of SendGrid.
            var sendGridConfiguration = applicationSetting.SendGridSetting;
            if (sendGridConfiguration == null)
                throw new Exception("Configuration is required");

            // Initialize an instance of SendGrid api client.
            _sendGridApiClient = new SendGridAPIClient(sendGridConfiguration.ApiKey);

            // Load the email contents.
            LoadEmailContentFromFile(httpContext, sendGridConfiguration.SendGridPreconfigurations);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for sending email contains activation code to client's email.
        /// </summary>
        /// <param name="destinations"></param>
        /// <param name="templateName"></param>
        /// <param name="data"></param>
        public async Task<bool> InitializeEmail(string[] destinations, string templateName, object data)
        {
            // Retrieve the content.
            var sendGridContent = ContentsCollection[templateName];

            // Render the email body.
            var body = Render.StringToString(sendGridContent.Body, data);

            // Address of broadcaster email.
            var from = new Email(sendGridContent.From);

            foreach (var destination in destinations)
            {
                try
                {
                    var to = new Email(destination);
                    var content = new Content("text/html", body);
                    var mail = new Mail(from, sendGridContent.Subject, to, content);

                    // Broadcast the email.
                    await _sendGridApiClient.client.mail.send.post(requestBody: mail.Get());
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                }
            }


            return true;
        }


        /// <summary>
        ///     Load email content from file.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="emailPreconfigurations"></param>
        private void LoadEmailContentFromFile(HttpContext httpContext,
            IDictionary<string, SendGridPreconfiguration> emailPreconfigurations)
        {
            // No email is found.
            if (emailPreconfigurations.Keys.Count < 1)
                return;

            // Go through every email setting and load its configuration.
            foreach (var key in emailPreconfigurations.Keys)
            {
                // Obtain the preconfiguration.
                var emailPreconfiguration = emailPreconfigurations[key];

                // Load the email.
                var filePath = httpContext.Server.MapPath(emailPreconfiguration.File);
                var content = File.ReadAllText(filePath);

                // Load email to template collection.
                var sendGridContent = new SendGridContent(emailPreconfiguration.Subject, content,
                    emailPreconfiguration.From);

                if (ContentsCollection.ContainsKey(key))
                {
                    ContentsCollection[key] = sendGridContent;
                    continue;
                }

                ContentsCollection.Add(key, sendGridContent);
            }
        }

        #endregion
    }
}