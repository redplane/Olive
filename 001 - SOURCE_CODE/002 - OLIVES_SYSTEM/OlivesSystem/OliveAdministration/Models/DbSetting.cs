using Newtonsoft.Json;
using Shared.Interfaces;

namespace DotnetSignalR.Models
{
    public class DbSetting : IDbSetting
    {
        [JsonProperty("db_url")]
        public string Url { get; set; }

        [JsonProperty("db_account")]
        public string Username { get; set; }

        [JsonProperty("db_password")]
        public string Password { get; set; }
    }
}