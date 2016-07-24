using System.Drawing;

namespace MultipartFormDataMediaFormatter.Models
{
    public class HttpImageModel
    {
        #region Properties

        /// <summary>
        ///     Parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Parameter value.
        /// </summary>
        public Image Value { get; set; }

        public string MediaType { get; set; }

        public byte[] Buffer { get; set; }

        #endregion
    }
}