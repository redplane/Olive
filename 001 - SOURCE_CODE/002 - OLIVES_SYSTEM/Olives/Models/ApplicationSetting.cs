using System.Collections.Generic;

namespace Olives.Models
{
    public class ApplicationSetting
    {
        /// <summary>
        ///     Simple mail transfer protocol setting properties.
        /// </summary>
        public SmtpSetting SmtpSetting { get; set; }

        /// <summary>
        ///     List of storage with their relative paths.
        /// </summary>
        public Dictionary<string, string> Storage { get; set; }
    }
}