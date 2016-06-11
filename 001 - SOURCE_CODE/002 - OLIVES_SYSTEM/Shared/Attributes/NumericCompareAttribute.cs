using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Shared.Enumerations;

namespace Shared.Attributes
{
    /// <summary>
    ///     This attribute is used for comparing 2 int typed attributes.
    /// </summary>
    public class NumericCompareAttribute : ValidationAttribute
    {
        #region Constructor

        /// <summary>
        /// Initialize an instance of NumericCompareAttribute class with long milestone.
        /// </summary>
        /// <param name="milestone"></param>
        public NumericCompareAttribute(long milestone)
        {
            Milestone = milestone;
        }

        /// <summary>
        /// Initialize an instance of NumericCompareAttribute class with integer milestone.
        /// </summary>
        /// <param name="milestone"></param>
        public NumericCompareAttribute(int milestone)
        {
            Milestone = milestone;
        }

        /// <summary>
        /// Initialize an instance of NumericCompareAttribute class with byte milestone.
        /// </summary>
        /// <param name="milestone"></param>
        public NumericCompareAttribute(byte milestone)
        {
            Milestone = milestone;
        }

        #endregion

        #region Property

        /// <summary>
        ///     Minimum value which property should be greater than.
        /// </summary>
        public long Milestone { get; set; }

        /// <summary>
        ///     Whether the compared property must be smaller than the source one or not.
        ///     null    : No smaller, no greater.
        ///     false   : No smaller,    greater.
        ///     true:   :    smaller, no greater.
        /// </summary>
        public Comparision Comparision { get; set; } = 0;

        #endregion

        #region Methods

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

            // Convert value to int.
            var convertedValue = (long) value;

            switch (Comparision)
            {
                case Comparision.Lower:
                {
                    if (convertedValue >= Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));

                    break;
                }
                case Comparision.LowerEqual:
                {
                    if (convertedValue > Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));
                    break;
                }
                case Comparision.Equal: // Value must be equal to milestone.
                {
                    if (convertedValue != Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));
                    break;
                }
                case Comparision.GreaterEqual: // Value must be larger than or equal to milestone.
                {
                    if (convertedValue < Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));
                    break;
                }
                case Comparision.Greater: // Value must be larger than milestone.
                {
                    if (convertedValue <= Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));
                    break;
                }
                default:
                    throw new NotImplementedException();
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Override format error message to support multi parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Milestone);
        }

        #endregion
    }
}