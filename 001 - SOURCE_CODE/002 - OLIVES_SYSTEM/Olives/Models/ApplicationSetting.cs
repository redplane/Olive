using Newtonsoft.Json;
using Shared.Models;

namespace Olives.Models
{
    public class ApplicationSetting
    {
        /// <summary>
        ///     Simple mail transfer protocol setting properties.
        /// </summary>
        public SmtpSetting SmtpSetting { get; set; }

        /// <summary>
        ///     Folder where avatar files should be stored.
        /// </summary>
        public PathService AvatarStorage { get; set; }

        /// <summary>
        ///     Folder where private files should be stored.
        /// </summary>
        public PathService MedicalImageStorage { get; set; }

        /// <summary>
        /// Folder where prescription images are stored.
        /// </summary>
        public PathService PrescriptionImageStorage { get; set; }
    }
}