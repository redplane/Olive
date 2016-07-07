using System;
using System.Drawing;
using System.IO;
using MultipartDataMediaFormatter.Infrastructure;

namespace Shared.Models
{
    public class HttpImageFile : HttpFile
    {
        /// <summary>
        /// Information of image.
        /// </summary>
        private Image _image;

        // By default, image hasn't been initialize.
        private bool _initialized;

        /// <summary>
        /// Get image information of image.
        /// </summary>
        public Image Image
        {
            get
            {
                // Information hasn't been retrieved.
                if (_initialized)
                    return _image;

                // Use memory stream to read bytes saved in buffer.
                try
                {
                    using (var memoryStream = new MemoryStream(Buffer))
                    {
                        // Seek the stream from the beginning.
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        _image = Image.FromStream(memoryStream);
                    }
                }
                catch (Exception)
                {
                    _image = null;
                }

                // Treat the image has been analyzed. No more analyzation should be done later.
                _initialized = true;
                return _image;
            }
        }

        public HttpImageFile() : base()
        {
            
        }
    }
}