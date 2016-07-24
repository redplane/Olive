using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MultipartFormDataMediaFormatter.Converters.Original;
using MultipartFormDataMediaFormatter.Helpers;
using MultipartFormDataMediaFormatter.Models;

namespace MultipartFormDataMediaFormatter.Converters
{
    public class HttpContentToMultipartFormDataConverter : HttpContentOriginalConverter
    {
        /// <summary>
        ///     This function is for converting HttpContent instance to FormFile instance.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<MultipartFormData> Convert(HttpContent content)
        {
            // Invalid content.
            if (content == null)
                return null;

            // Content is unsupported.
            if (!content.IsMimeMultipartContent())
                throw new Exception("Unsupported Media Type");

            // Read stream provider from content.
            var multipartMemoryStreamProvider = await content.ReadAsMultipartAsync();

            // Retrieve the multipart form data from provider.
            var formFile = await Convert(multipartMemoryStreamProvider);
            return formFile;
        }

        /// <summary>
        ///     Convert MultipartMemoryStreamProvider instance to FormFile.
        /// </summary>
        /// <param name="multipartProvider"></param>
        /// <returns></returns>
        public async Task<MultipartFormData> Convert(MultipartMemoryStreamProvider multipartProvider)
        {
            // Initialize an instance from form file.
            var formFile = new MultipartFormData();

            // Loop through every content to put file into FormFile instance.
            foreach (var file in multipartProvider.Contents.Where(x => IsFile(x.Headers.ContentDisposition)))
            {
                var name = RemoveQuotes(file.Headers.ContentDisposition.Name);
                var fileName = FixFilename(file.Headers.ContentDisposition.FileName);
                var mediaType = file.Headers.ContentType.MediaType;

                using (var stream = await file.ReadAsStreamAsync())
                {
                    var buffer = ReadAllBytes(stream);
                    if (buffer.Length > 0)
                    {
                        // Parameter is the image.
                        if (ConstantHelper.Instance.ImageMediaTypes.Any(x => x.Equals(mediaType)))
                        {
                            try
                            {
                                using (var memoryStream = new MemoryStream(buffer))
                                {
                                    memoryStream.Seek(0, SeekOrigin.Begin);
                                    var image = Image.FromStream(memoryStream);

                                    var httpImage = new HttpImageModel();
                                    httpImage.Name = name;
                                    httpImage.Value = image;
                                    httpImage.Buffer = buffer;
                                    httpImage.MediaType = mediaType;

                                    formFile.Add(httpImage);
                                }
                            }
                            catch
                            {
                                // Suppress error as image analyzation is failed.
                            }

                            continue;
                        }

                        formFile.Add(name, new HttpFileModel(fileName, mediaType, buffer));
                    }
                }
            }

            // Loop through every content to put data into FormFile instance.
            foreach (
                var part in
                    multipartProvider.Contents.Where(x => x.Headers.ContentDisposition.DispositionType == "form-data"
                                                          && !IsFile(x.Headers.ContentDisposition)))
            {
                var name = RemoveQuotes(part.Headers.ContentDisposition.Name);
                var data = await part.ReadAsStringAsync();
                formFile.Add(name, data);
            }

            return formFile;
        }
    }
}