using Newtonsoft.Json;

namespace Olives.Models
{
    public class ApplicationSetting
    {
        /// <summary>
        /// Simple mail transfer protocol setting properties.
        /// </summary>
        [JsonProperty("smtpSetting")]
        public SmtpSetting SmtpSetting { get; set; }

        /// <summary>
        /// Folder where public files should be stored.
        /// </summary>
        [JsonProperty("publicStorage")]
        public string PublicStorage { get; set; }
    }
}