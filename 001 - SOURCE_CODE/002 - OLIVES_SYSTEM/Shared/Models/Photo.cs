namespace Shared.Models
{
    public class Photo
    {
        /// <summary>
        /// Url of photo.
        /// </summary>
        public string Relative { get; set; }

        /// <summary>
        /// Physical path of photo.
        /// </summary>
        public string Absolute { get; set; }
    }
}