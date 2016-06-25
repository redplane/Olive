using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Shared.Enumerations;

namespace Shared.Attributes
{
    public class InAccountStatusAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Values collection in which data must be equal.
        /// </summary>
        private readonly AccountStatus[] _milesStone;

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

            if (!(value is AccountStatus))
                throw new Exception("Invalid account status");

            // Invalid milestone.
            if (_milesStone == null || _milesStone.Length < 1)
                throw new Exception("Invalid milestones.");

            // Cast value to status.
            var status = (AccountStatus) value;

            if (!_milesStone.Any(x => x == status))
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
            var milestoneList = string.Join(",", _milesStone);
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, milestoneList);
        }

        #region Constructor

        /// <summary>
        ///     Initialize an instance of IntsMatchAttribute class.
        /// </summary>
        /// <param name="milestones"></param>
        public InAccountStatusAttribute(AccountStatus[] milestones)
        {
            _milesStone = milestones;
        }
        
        #endregion
    }
}