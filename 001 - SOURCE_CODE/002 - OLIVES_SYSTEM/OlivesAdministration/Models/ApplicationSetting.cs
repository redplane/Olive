using System.Collections.Generic;

namespace OlivesAdministration.Models
{
    public class ApplicationSetting
    {
        /// <summary>
        ///     List of storage with their relative paths.
        /// </summary>
        public Dictionary<string, string> Storage { get; set; }
    }
}