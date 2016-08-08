using Newtonsoft.Json;

namespace Shared.Models
{
    public class PathService
    {
        /// <summary>
        ///     The relative path is for url construction sent to client.
        /// </summary>
        [JsonProperty(PropertyName = "relative")]
        public string Relative { get; set; }

        /// <summary>
        ///     Absolute path is for file management purpose.
        /// </summary>
        [JsonProperty(PropertyName = "absolute")]
        public string Absolute { get; set; }
    }
}