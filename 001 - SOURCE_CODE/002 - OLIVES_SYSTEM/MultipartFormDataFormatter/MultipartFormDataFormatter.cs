using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MultipartFormDataMediaFormatter.Analyzers;
using MultipartFormDataMediaFormatter.Converters;

namespace MultipartFormDataMediaFormatter
{
    public class MultipartFormDataFormatter : MediaTypeFormatter
    {
        #region Properties

        /// <summary>
        ///     Supported media type.
        /// </summary>
        private const string SupportedMediaType = "multipart/form-data";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance with default settings.
        /// </summary>
        public MultipartFormDataFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(SupportedMediaType));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Every instance can be read.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanReadType(Type type)
        {
            return true;
        }

        /// <summary>
        ///     Every instance can be written.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanWriteType(Type type)
        {
            return true;
        }

        /// <summary>
        ///     Initialize boundary for content headers.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="headers"></param>
        /// <param name="mediaType"></param>
        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers,
            MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);

            if (headers.ContentType == null)
                headers.ContentType = new MediaTypeHeaderValue(SupportedMediaType);

            if (!string.Equals(headers.ContentType.MediaType, SupportedMediaType, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Not a Multipart Content");

            if (headers.ContentType.Parameters.All(m => m.Name != "boundary"))
                headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary",
                    "MultipartDataMediaFormatterBoundary1q2w3e"));
        }

        /// <summary>
        ///     Read data from request stream.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="readStream"></param>
        /// <param name="content"></param>
        /// <param name="formatterLogger"></param>
        /// <returns></returns>
        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            var httpContentToFormDataConverter = new HttpContentToMultipartFormDataConverter();
            var multipartFormData = await httpContentToFormDataConverter.Convert(content);

            var dataToObjectConverter = new MultipartFormFileAnalyzer(multipartFormData);
            var result = dataToObjectConverter.Convert(type);

            return result;
        }

        /// <summary>
        ///     Write data to response stream.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="writeStream"></param>
        /// <param name="content"></param>
        /// <param name="transportContext"></param>
        /// <returns></returns>
        public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            if (!content.IsMimeMultipartContent())
                throw new Exception("Not a Multipart Content");

            var boudaryParameter =
                content.Headers.ContentType.Parameters.FirstOrDefault(
                    m => m.Name == "boundary" && !string.IsNullOrWhiteSpace(m.Value));
            if (boudaryParameter == null)
                throw new Exception("Multipart boundary not found");

            var objectToMultipartDataByteArrayConverter = new MultipartFormDataAnalyzer();
            var multipartData = objectToMultipartDataByteArrayConverter.Analyze(value, boudaryParameter.Value);

            await writeStream.WriteAsync(multipartData, 0, multipartData.Length);

            content.Headers.ContentLength = multipartData.Length;
        }

        #endregion
    }
}