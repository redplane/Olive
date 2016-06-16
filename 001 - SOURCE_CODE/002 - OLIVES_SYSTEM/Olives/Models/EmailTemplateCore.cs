
using Newtonsoft.Json;

namespace Olives.Models
{
    public class EmailTemplateCore
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        
        [JsonProperty("isHtml")]
        public bool IsHtml { get; set; }
        
        [JsonProperty("subject")]
        public string Subject { get; set; } 
    }
}