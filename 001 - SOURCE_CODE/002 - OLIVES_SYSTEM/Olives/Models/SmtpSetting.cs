using Newtonsoft.Json;

namespace Olives.Models
{
    public class SmtpSetting
    {
        [JsonProperty("host")]
        public string Host { get; set; } 

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("enableSSL")]
        public bool EnableSsl { get; set; }

        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Host))
                return false;

            if (string.IsNullOrEmpty(Email))
                return false;

            if (string.IsNullOrEmpty(Password))
                return false;

            return true;
        }
    }
}