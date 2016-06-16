using Newtonsoft.Json;
using Shared.Models;

namespace Olives.Models
{
    public class ApplicationSetting
    {
        [JsonProperty("database")]
        public DbSetting Database { get; set; }

        [JsonProperty("smtpSetting")]
        public SmtpSetting SmtpSetting { get; set; }
    }
}