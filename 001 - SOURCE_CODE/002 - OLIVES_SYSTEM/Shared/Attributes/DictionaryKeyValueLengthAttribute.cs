using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Shared.Attributes
{
    public class DictionaryKeyValueLengthAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Length of key.
        /// </summary>
        private readonly int _keyLength;

        /// <summary>
        ///     Length of value.
        /// </summary>
        private readonly int _valueLength;

        /// <summary>
        ///     Initialize an instance of RegexMatchAttribute class.
        /// </summary>
        /// <param name="keyLength"></param>
        /// <param name="valueLength"></param>
        public DictionaryKeyValueLengthAttribute(int keyLength, int valueLength)
        {
            _keyLength = keyLength;
            _valueLength = valueLength;
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
            var dict = (Dictionary<string, string>) value;

            // Key length is defined.
            if (_keyLength > 0)
            {
                // Find the invalid key.
                var invalidKey = dict.Keys.FirstOrDefault(x => x.Length > _keyLength);
                if (invalidKey != null)
                    return new ValidationResult(FormatKeyErrorMessage(invalidKey));
            }

            // Value length is defined.
            if (_valueLength > 0)
            {
                // Find the invalid value.
                var invalidValue = dict.Values.FirstOrDefault(x => x.Length > _valueLength);
                if (invalidValue != null)
                    return new ValidationResult(FormatKeyValueErrorMessage(invalidValue));
            }
            return ValidationResult.Success;
        }

        /// <summary>
        ///     Override format error message to support multi parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string FormatKeyErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _keyLength);
        }

        /// <summary>
        ///     Override format error message to support multi parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string FormatKeyValueErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _valueLength);
        }

    }
}