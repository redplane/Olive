using Newtonsoft.Json;

namespace Olives.Models
{
    public class SmtpSetting
    {
        #region Methods

        /// <summary>
        ///     Check whether smtp configuration is valid or not.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // Host hasn't been defined.
            if (string.IsNullOrEmpty(Host))
                return false;

            // Email hasn't been defined.
            if (string.IsNullOrEmpty(Email))
                return false;

            // Password hasn't been defined.
            if (string.IsNullOrEmpty(Password))
                return false;

            // Invalid port.
            if (Port != 465 && Port != 587)
                return false;

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Smtp host server.
        /// </summary>
        [JsonProperty("host")]
        public string Host { get; set; }

        /// <summary>
        ///     Smtp port.
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }

        /// <summary>
        ///     Whether SSL connection is enabled.
        /// </summary>
        [JsonProperty("enableSSL")]
        public bool EnableSsl { get; set; }

        /// <summary>
        ///     How much time connection has to wait before being cancelled.
        /// </summary>
        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        /// <summary>
        ///     Email which used for sending messages
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        ///     Password of email.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("emailTemplates")]
        public EmailTemplate[] EmailTemplates { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        #endregion
    }
}