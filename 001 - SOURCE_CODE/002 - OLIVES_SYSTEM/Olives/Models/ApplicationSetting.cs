using Newtonsoft.Json;

namespace Olives.Models
{
    public class ApplicationSetting
    {
        /// <summary>
        ///     Simple mail transfer protocol setting properties.
        /// </summary>
        [JsonProperty("smtpSetting")]
        public SmtpSetting SmtpSetting { get; set; }

        /// <summary>
        ///     Folder where avatar files should be stored.
        /// </summary>
        [JsonProperty("avatarStorage")]
        public ServerPath AvatarStorage { get; set; }

        /// <summary>
        ///     Folder where private files should be stored.
        /// </summary>
        [JsonProperty("privateStorage")]
        public ServerPath PrivateStorage { get; set; }
    }
}