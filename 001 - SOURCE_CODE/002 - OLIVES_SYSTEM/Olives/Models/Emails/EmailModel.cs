namespace Olives.Models.Emails
{
    public class EmailModel
    {
        #region Properties

        /// <summary>
        /// Subject of email.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Body of email.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Whether email should be written in html or not.
        /// </summary>
        public bool IsHtml { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize an model of email with defined parameters.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        public EmailModel(string subject, string body, bool isHtml)
        {
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
        }

        #endregion
    }
}