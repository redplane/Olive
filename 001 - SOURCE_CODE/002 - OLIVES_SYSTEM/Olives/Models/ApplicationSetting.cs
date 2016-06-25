using Newtonsoft.Json;
using Shared.Models;

namespace Olives.Models
{
    public class ApplicationSetting
    {
        [JsonProperty("smtpSetting")]
        public SmtpSetting SmtpSetting { get; set; }
    }
}