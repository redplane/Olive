namespace MultipartDataMediaFormatter.Infrastructure
{
    public class HttpFile
    {
        /// <summary>
        ///     Name of file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Type of file.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        ///     Serialized byte stream of file.
        /// </summary>
        public byte[] Buffer { get; set; }

        #region Constructors

        /// <summary>
        ///     Initialize an instance of HttpFile without any setting.
        /// </summary>
        public HttpFile()
        {
        }

        /// <summary>
        ///     Initialize an instance of HttpFile with parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mediaType"></param>
        /// <param name="buffer"></param>
        public HttpFile(string fileName, string mediaType, byte[] buffer)
        {
            FileName = fileName;
            MediaType = mediaType;
            Buffer = buffer;
        }

        #endregion
    }
}