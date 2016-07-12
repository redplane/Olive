using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.ViewModels;

namespace Olives.Attributes
{
    public class MedicineListLengthValidateAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Maximum key is allowed.
        /// </summary>
        private readonly int _maxKey;

        /// <summary>
        ///     Initialize an instance of MedicineListLengthValidateAttribute.
        /// </summary>
        /// <param name="maxKey"></param>
        public MedicineListLengthValidateAttribute(int maxKey)
        {
            _maxKey = maxKey;
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
            if (!(value is Dictionary<string, MedicineInfoViewModel>))
                throw new Exception(
                    $"{validationContext} must be an instance of Dictionary<string, MedicineInfoViewModel>");

            // Cast the value to Dictionary<string, string>()
            var dict = (Dictionary<string, MedicineInfoViewModel>) value;

            // Key length is defined.
            if (_maxKey > 0)
                if (dict.Keys.Count > _maxKey)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}