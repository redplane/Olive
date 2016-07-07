using MultipartDataMediaFormatter.Infrastructure;

namespace MultipartDataMediaFormatter.Models
{
    public class FileModel
    {
        /// <summary>
        /// Name of file parameter sent to server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of parameter.
        /// </summary>
        public HttpFile Value { get; set; }
    }
}