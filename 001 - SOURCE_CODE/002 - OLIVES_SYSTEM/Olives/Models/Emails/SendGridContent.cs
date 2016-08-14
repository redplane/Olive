namespace Olives.Models.Emails
{
    public class SendGridContent
    {
        #region Constructors

        /// <summary>
        ///     Initialize an model of email with defined parameters.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="from"></param>
        public SendGridContent(string subject, string body, string from)
        {
            Subject = subject;
            Body = body;
            From = from;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Subject of email.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        ///     Body of email.
        /// </summary>
        public string Body { get; private set; }
        
        /// <summary>
        /// Address where email sent from.
        /// </summary>
        public string From { get; private set; }

        #endregion
    }
}