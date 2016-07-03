using Newtonsoft.Json;

namespace Olives.Models
{
    public class ApplicationSetting
    {
        [JsonProperty("smtpSetting")]
        public SmtpSetting SmtpSetting { get; set; }
    }
}