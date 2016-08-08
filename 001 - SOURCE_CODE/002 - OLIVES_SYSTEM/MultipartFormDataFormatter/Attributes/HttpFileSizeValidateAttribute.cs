using System;
using System.ComponentModel.DataAnnotations;
using MultipartFormDataMediaFormatter.Models;

namespace MultipartFormDataMediaFormatter.Attributes
{
    public class HttpFileSizeValidateAttribute : ValidationAttribute
    {
        #region Properties

        /// <summary>
        ///     Allowed content length of the file.
        /// </summary>
        private readonly uint _contentLength;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize an instance of MedicineListLengthValidateAttribute.
        /// </summary>
        /// <param name="bytes"></param>
        public HttpFileSizeValidateAttribute(uint bytes)
        {
            _contentLength = bytes;
        }

        #endregion

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
            if (httpFile.Buffer.Length > _contentLength)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}