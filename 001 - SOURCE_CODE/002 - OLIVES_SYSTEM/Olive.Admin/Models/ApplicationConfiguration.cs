using ArangoDB.Client;

namespace OliveAdmin.Models
{
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Shared setting of database.
        /// </summary>
        public DatabaseSharedSetting DatabaseSharedSetting { get; set; }
    }
}