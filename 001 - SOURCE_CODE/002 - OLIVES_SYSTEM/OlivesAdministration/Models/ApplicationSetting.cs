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
        public PathService AvatarStorage { get; set; }

        /// <summary>
        ///     Folder where private files should be stored.
        /// </summary>
        [JsonProperty("privateStorage")]
        public PathService PrivateStorage { get; set; }
    }
}