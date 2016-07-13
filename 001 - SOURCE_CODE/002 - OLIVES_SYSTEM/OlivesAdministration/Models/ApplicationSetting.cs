using Newtonsoft.Json;

namespace OlivesAdministration.Models
{
    public class ApplicationSetting
    {
        /// <summary>
        ///     Folder where avatar files should be stored.
        /// </summary>
        [JsonProperty("avatarStorage")]
        public Shared.Models.ServerPath AvatarStorage { get; set; }

        /// <summary>
        ///     Folder where private files should be stored.
        /// </summary>
        [JsonProperty("privateStorage")]
        public Shared.Models.ServerPath PrivateStorage { get; set; }
    }
}