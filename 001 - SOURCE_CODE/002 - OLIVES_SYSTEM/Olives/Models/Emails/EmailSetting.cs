namespace Olives.Models.Emails
{
    public class EmailSetting
    {
        #region Properties

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// File which contains email content.
        /// </summary>
        public string File { get; set; }
        
        /// <summary>
        /// Whether email is written in html.
        /// </summary>
        public bool IsHtml { get; set; }

        #endregion
    }
}