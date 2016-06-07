using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Shared.Attributes
{
    /// <summary>
    ///     This attribute is used for comparing 2 int typed attributes.
    /// </summary>
    public class IntsMatchAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Values collection in which data must be equal.
        /// </summary>
        private readonly int[] _milesStone;

        /// <summary>
        ///     Initialize an instance of IntsMatchAttribute class.
        /// </summary>
        /// <param name="milestones"></param>
        public IntsMatchAttribute(int[] milestones)
        {
            _milesStone = milestones;
        }

        /// <summary>
        ///     Check whether property is valid or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Value is null. This mean no validation is specified.
            if (value == null)
                return ValidationResult.Success;

            // Invalid milestone.
            if (_milesStone == null || _milesStone.Length < 1)
                throw new Exception("Invalid milestones.");


            // Convert value to int.
            var convertedValue = (int) value;

            if (!_milesStone.Any(x => x == convertedValue))
            {
                var milestoneList = string.Join(",", _milesStone);
                return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName, milestoneList));
            }

            return ValidationResult.Success;
        }
    }
}