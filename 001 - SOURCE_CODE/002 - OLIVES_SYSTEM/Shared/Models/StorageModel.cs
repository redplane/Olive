namespace Shared.Models
{
    public class StorageModel
    {
        /// <summary>
        /// Relative path of storage.
        /// </summary>
        public string Relative { get; set; }
        
        /// <summary>
        /// Absolute (physical) path of storage.
        /// </summary>
        public string Absolute { get; set; } 
    }
}