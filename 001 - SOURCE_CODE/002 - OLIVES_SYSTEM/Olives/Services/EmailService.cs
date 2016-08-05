using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Nustache.Core;
using Olives.Constants;
using Olives.Interfaces;
using Olives.Models;
using Olives.Models.Emails;
using Shared.Models;

namespace Olives.Services
{
    public class EmailService : IEmailService
    {
        #region Properties

        /// <summary>
        ///     Smtp client instance which is used for sending email to clients.
        /// </summary>
        private readonly SmtpClient _smtpClient;

        /// <summary>
        ///     Configuration of stmp client.
        /// </summary>
        private readonly SmtpSetting _stmpConfiguration;

        /// <summary>
        ///     Collection of email templates.
        /// </summary>
        public Dictionary<string, EmailModel> EmailTemplatesCollection { get; set; }

        #endregion

        #region Constructor

        public EmailService()
        {
        }

        /// <summary>
        ///     Initialize an instance of Email service with specific configurations.
        /// </summary>
        /// <param name="config"></param>
        public EmailService(SmtpSetting config)
        {
            _smtpClient = new SmtpClient();
            _smtpClient.Port = config.Port;
            _smtpClient.Host = config.Host;
            _smtpClient.EnableSsl = config.EnableSsl;
            _smtpClient.Timeout = config.Timeout;
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = new NetworkCredential(config.Email, config.Password);
            _stmpConfiguration = config;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for sending email contains activation code to client's email.
        /// </summary>
        /// <param name="destinations"></param>
        /// <param name="templateName"></param>
        /// <param name="data"></param>
        public async Task<bool> InitializeEmail(string [] destinations, string templateName, object data)
        {
            // Find the email model by using template name.
            var emailModel = EmailTemplatesCollection[templateName];

            // Render email body from template with given information.
            var render = Render.StringToString(emailModel.Body, data);

            // Initialize mail message.
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_stmpConfiguration.From, _stmpConfiguration.DisplayName);
            foreach (var destination in destinations)
                mailMessage.To.Add(new MailAddress(destination));
            mailMessage.Subject = emailModel.Subject;
            mailMessage.Body = render;
            mailMessage.BodyEncoding = Encoding.UTF32;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            mailMessage.IsBodyHtml = emailModel.IsHtml;
            await _smtpClient.SendMailAsync(mailMessage);
            return true;
        }

        #endregion
    }
}