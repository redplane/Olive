using System;
using System.IO;
using Olives.Interfaces;

namespace Olives.Services
{
    public class FileService : IFileService
    {
        /// <summary>
        /// Encode a file to base64 format string.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string EncodeFileBase64(string fullPath)
        {
            // Invalid path.
            if (!string.IsNullOrWhiteSpace(fullPath))
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