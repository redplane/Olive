using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MultipartFormDataMediaFormatter.Models;

namespace MultipartFormDataMediaFormatter.Attributes
{
    public class HttpFileMediatypeValidateAttribute : ValidationAttribute
    {
        #region Properties

        /// <summary>
        ///     Allowed content length of the file.
        /// </summary>
        private readonly HashSet<string> _mediaTypes;

        #endregion

        #region Methods

        /// <summary>
        ///     Check whether regular expression is valid or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Invalid value.
            if (value == null)
                return ValidationResult.Success;

            if (!(value is HttpFileModel))
                throw new Exception("Object my be an instance of HttpFileModel");

            // Cast object to HttpFileModel instance.
            var httpFile = (HttpFileModel) value;

            // The media type of file is not supported.
            if (!_mediaTypes.Any(x => x.Equals(httpFile.MediaType, StringComparison.InvariantCultureIgnoreCase)))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize an instance of HttpFileMediatypeValidateAttribute.
        /// </summary>
        public HttpFileMediatypeValidateAttribute()
        {
            _mediaTypes = new HashSet<string>();
        }

        /// <summary>
        ///     Initialize an instance of HttpFileMediatypeValidateAttribute.
        /// </summary>
        /// <param name="mediaType"></param>
        public HttpFileMediatypeValidateAttribute(string mediaType) : this()
        {
            _mediaTypes.Add(mediaType);
        }

        /// <summary>
        ///     Initialize an instance of HttpFileMediatypeValidateAttribute
        /// </summary>
        /// <param name="mediaTypes"></param>
        public HttpFileMediatypeValidateAttribute(string[] mediaTypes)
        {
            foreach (var mediaType in mediaTypes)
            {
                if (_mediaTypes.Contains(mediaType))
                    continue;

                _mediaTypes.Add(mediaType);
            }
        }

        /// <summary>
        ///     Initialize an instance of HttpFileMediatypeValidateAttribute
        /// </summary>
        /// <param name="mediaTypes"></param>
        public HttpFileMediatypeValidateAttribute(IList<string> mediaTypes)
        {
            foreach (var mediaType in mediaTypes)
            {
                if (_mediaTypes.Contains(mediaType))
                    continue;

                _mediaTypes.Add(mediaType);
            }
        }

        #endregion
    }
}