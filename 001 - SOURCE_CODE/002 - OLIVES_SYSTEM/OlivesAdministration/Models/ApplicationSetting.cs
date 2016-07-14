using Newtonsoft.Json;
using Shared.Models;

namespace OlivesAdministration.Models
{
    public class ApplicationSetting
    {
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