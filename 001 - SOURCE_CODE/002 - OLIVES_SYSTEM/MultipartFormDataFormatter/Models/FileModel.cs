namespace MultipartFormDataMediaFormatter.Models
{
    public class FileModel
    {
        /// <summary>
        ///     Name of file parameter sent to server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Value of parameter.
        /// </summary>
        public HttpFileModel Value { get; set; }
    }
}