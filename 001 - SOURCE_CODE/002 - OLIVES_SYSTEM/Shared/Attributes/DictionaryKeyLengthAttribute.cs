using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Shared.Attributes
{
    public class DictionaryKeyLengthAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Length of key.
        /// </summary>
        private readonly int _keyLength;

        /// <summary>
        ///     Initialize an instance of RegexMatchAttribute class.
        /// </summary>
        /// <param name="keyLength"></param>
        public DictionaryKeyLengthAttribute(int keyLength)
        {
            _keyLength = keyLength;
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
            var dict = (Dictionary<string, object>) value;

            // Key length is defined.
            if (_keyLength > 0)
            {
                // Find the invalid key.
                var invalidKey = dict.Keys.FirstOrDefault(x => x.Length > _keyLength);
                if (invalidKey != null)
                    return new ValidationResult(FormatErrorMessage(invalidKey));
            }

            return ValidationResult.Success;
        }
    }
}