using System.Net.Mail;
using Olives.Models;

namespace Olives.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        ///     Send email with specific information.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        void SendEmail(string from, string to, string subject, string body);

        /// <summary>
        ///     Send email with specific information.
        /// </summary>
        /// <param name="mailMessage"></param>
        void SendEmail(MailMessage mailMessage);

        ///// <summary>
        /////     This function is for sending activation code to a specific client.
        ///// </summary>
        ///// <param name="to"></param>
        ///// <param name="subject"></param>
        ///// <param name="firstName"></param>
        ///// <param name="lastName"></param>
        ///// <param name="activationCode"></param>
        //void SendActivationCode(string to, string subject, string firstName, string lastName,
        //    ActivationCode activationCode);

        /// <summary>
        ///     Load email template from specific files.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        bool LoadEmailTemplate(string name, string file, EmailTemplateCore core);
    }
}