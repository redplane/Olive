using Newtonsoft.Json;
using Shared.Interfaces;

namespace Shared.Models
{
    public class DbSetting : IDbSetting
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("account")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}