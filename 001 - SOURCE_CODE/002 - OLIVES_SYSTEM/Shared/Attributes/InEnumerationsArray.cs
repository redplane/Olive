using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Shared.Attributes
{
    public class InEnumerationsArray : ValidationAttribute
    {
        /// <summary>
        ///     Values collection in which data must be equal.
        /// </summary>
        private readonly int[] _milesStone;

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
            var casted = (int) value;

            if (!_milesStone.Any(x => x.Equals(casted)))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        /// <summary>
        ///     Override format error message to support multi parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            // convert array to string array.
            var milestoneList = string.Join(",", _milesStone);

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, milestoneList);
        }

        #region Constructor

        /// <summary>
        ///     Initialize an instance of IntsMatchAttribute class.
        /// </summary>
        /// <param name="milestones"></param>
        public InEnumerationsArray(object[] milestones)
        {
            _milesStone = Array.ConvertAll(milestones, value => (int)value);
        }
        
        #endregion
    }
}