namespace MultipartFormDataMediaFormatter.Models
{
    public class HttpFileModel
    {
        #region Properties

        /// <summary>
        ///     Name of file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Type of file.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        ///     Serialized byte stream of file.
        /// </summary>
        public byte[] Buffer { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of HttpFile without any setting.
        /// </summary>
        public HttpFileModel()
        {
        }

        /// <summary>
        ///     Initialize an instance of HttpFile with parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mediaType"></param>
        /// <param name="buffer"></param>
        public HttpFileModel(string fileName, string mediaType, byte[] buffer)
        {
            Name = fileName;
            MediaType = mediaType;
            Buffer = buffer;
        }

        #endregion
    }
}