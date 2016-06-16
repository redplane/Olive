using Newtonsoft.Json;

namespace Olives.Models
{
    public class EmailTemplate
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("core")]
        public EmailTemplateCore Core { get; set; }
    }
}