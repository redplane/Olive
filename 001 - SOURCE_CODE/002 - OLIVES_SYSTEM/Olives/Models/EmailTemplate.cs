using Newtonsoft.Json;

namespace Olives.Models
{
    public class EmailTemplate
    {
        [JsonProperty("name")]
        public string Name { get; set; } 

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}