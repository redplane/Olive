using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes
{
    public class DictionaryLengthAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Length of key.
        /// </summary>
        private readonly int _length;

        /// <summary>
        ///     Initialize an instance of RegexMatchAttribute class.
        /// </summary>
        /// <param name="length"></param>
        public DictionaryLengthAttribute(int length)
        {
            _length = length;
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

            // Value is not a Dictionary<string, string>.
            if (!(value is IDictionary))
                throw new Exception($"{validationContext} must be an instance of Dictionary<string, string>");

            // Cast the value to Dictionary<string, string>()
            var dict = (IDictionary) value;

            // Key length is defined.
            if (dict.Keys.Count > _length)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}