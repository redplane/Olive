using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Olives.Interfaces;
using Olives.Models;

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
        public Dictionary<string, EmailTemplateCore> Templates { get; }

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

            Templates = new Dictionary<string, EmailTemplateCore>();
            _stmpConfiguration = config;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Send email with specific information.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendEmail(string from, string to, string subject, string body)
        {
            var mailMessage = new MailMessage(from, to, subject, body);
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            _smtpClient.Send(mailMessage);
        }

        /// <summary>
        ///     Send email with specific information.
        /// </summary>
        /// <param name="mailMessage"></param>
        public void SendEmail(MailMessage mailMessage)
        {
            _smtpClient.Send(mailMessage);
        }

        /// <summary>
        ///     Load email template from specific files.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <param name="core"></param>
        /// <returns></returns>
        public bool LoadEmailTemplate(string name, string file, EmailTemplateCore core)
        {
            // File doesn't exist.
            if (!File.Exists(file))
                return false;

            try
            {
                var info = File.ReadAllText(file);
                core.Content = info;
                Templates[name] = core;
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        ///// <summary>
        /////     This function is for sending email contains activation code to client's email.
        ///// </summary>
        ///// <param name="to"></param>
        ///// <param name="subject"></param>
        ///// <param name="firstName"></param>
        ///// <param name="lastName"></param>
        ///// <param name="activationCode"></param>
        //public void SendActivationCode(string to, string subject, string firstName, string lastName,
        //    ActivationCode activationCode)
        //{
        //    // Calculate the time when code will be expired.
        //    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    epoch = epoch.AddMilliseconds(activationCode.Expire);

        //    var data = new
        //    {
        //        firstName,
        //        lastName,
        //        code = activationCode.Code,
        //        date = epoch
        //    };

        //    //// Render email body from template with given information.
        //    var render = Render.StringToString(Templates[EmailType.Activation].Content, data);

        //    // Initialize mail message.
        //    var mailMessage = new MailMessage();
        //    mailMessage.From = new MailAddress(_stmpConfiguration.From, _stmpConfiguration.DisplayName);
        //    mailMessage.To.Add(new MailAddress(to));
        //    mailMessage.Subject = Templates[EmailType.Activation].Subject;
        //    mailMessage.Body = render;
        //    mailMessage.BodyEncoding = Encoding.UTF8;
        //    mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        //    mailMessage.IsBodyHtml = Templates[EmailType.Activation].IsHtml;
        //    SendEmail(mailMessage);
        //}

        #endregion
    }
}