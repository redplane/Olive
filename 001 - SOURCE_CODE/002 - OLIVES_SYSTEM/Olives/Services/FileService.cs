using System;
using System.Drawing;
using System.IO;
using Olives.Interfaces;

namespace Olives.Services
{
    public class FileService : IFileService
    {
        /// <summary>
        /// Convert a stream of bytes to Image instance.
        /// </summary>
        /// <param name="byteStream"></param>
        /// <returns></returns>
        public Image ConvertBytesToImage(byte[] byteStream)
        {
            try
            {
                var memoryStream = new MemoryStream(byteStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return Image.FromStream(memoryStream);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Encode a file to base64 format string.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string EncodeFileBase64(string fullPath)
        {
            // Invalid path.
            if (string.IsNullOrWhiteSpace(fullPath))
                return null;

            // File doesn't exist.
            if (!File.Exists(fullPath))
                return null;

            // Encode the file.
            var fileStream = File.ReadAllBytes(fullPath);
            var encodedData = Convert.ToBase64String(fileStream);
            return encodedData;
        }
    }
}