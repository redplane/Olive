using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Shared.Attributes
{
    public class RegexMatchAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Initialize an instance of RegexMatchAttribute class.
        /// </summary>
        /// <param name="pattern"></param>
        public RegexMatchAttribute(string pattern)
        {
            Pattern = pattern;
        }

        /// <summary>
        ///     RegularExpression pattern.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        ///     Option of regular expression.
        /// </summary>
        public RegexOptions Options { get; set; }

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

            // Value is not a string.
            if (!(value is string))
                return ValidationResult.Success;

            var input = (string) value;

            if (Regex.IsMatch(input, Pattern, Options))
                return ValidationResult.Success;

            return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
        }
    }
}