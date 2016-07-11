using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Shared.Attributes
{
    public class ValueStringLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// Maximum length that a string can contain.
        /// </summary>
        private readonly int _maxLength;

        /// <summary>
        /// Initialize an instance of ValueStringLengthAttribute with given parameters.
        /// </summary>
        /// <param name="maxLength"></param>
        public ValueStringLengthAttribute(int maxLength)
        {
            _maxLength = maxLength;
        }

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
            
            if (!(value is string))
                throw new Exception("Value must be a string");

            // Cast the value to an instance of string.
            var castedInstance = (string) value;
            if (castedInstance.Length > _maxLength)
                return new ValidationResult(FormatErrorMessage(castedInstance));

            return ValidationResult.Success;
        }

        /// <summary>
        ///     Override format error message to support multi parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _maxLength);
        }

    }
}