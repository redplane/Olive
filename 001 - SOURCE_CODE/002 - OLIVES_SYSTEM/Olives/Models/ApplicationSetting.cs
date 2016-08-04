using Newtonsoft.Json;
using Shared.Models;

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
        public PathService AvatarStorage { get; set; }

        /// <summary>
        ///     Folder where private files should be stored.
        /// </summary>
        [JsonProperty("medicalImageStorage")]
        public PathService MedicalImageStorage { get; set; }

        [JsonProperty("prescriptionImageStorage")]
        public PathService PrescriptionStorage { get; set; }
    }
}