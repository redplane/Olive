using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Olives.ViewModels;

namespace Olives.Attributes
{
    public class MedicineListKeyValidateAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Maximum length of name.
        /// </summary>
        public int MaxLengthName { get; set; }

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
            if (!(value is Dictionary<string, MedicineInfoViewModel>))
                throw new Exception(
                    $"{validationContext} must be an instance of Dictionary<string, MedicineInfoViewModel>");

            // Cast the value to Dictionary<string, string>()
            var dict = (Dictionary<string, MedicineInfoViewModel>) value;

            // Key length is defined.
            if (MaxLengthName > 0)
            {
                var invalidKey = dict.Keys.FirstOrDefault(x => x.Length > MaxLengthName);
                if (invalidKey != null)
                    return new ValidationResult(FormatErrorMessage(invalidKey));
            }

            return ValidationResult.Success;
        }
    }
}