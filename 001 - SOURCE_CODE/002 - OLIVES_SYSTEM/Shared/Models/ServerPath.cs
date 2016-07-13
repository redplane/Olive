using Newtonsoft.Json;

namespace Shared.Models
{
    public class ServerPath
    {
        [JsonProperty(PropertyName = "relative")]
        public string Relative { get; set; }

        [JsonProperty(PropertyName = "absolute")]
        public string Absolute { get; set; }
    }
}