using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes
{
    /// <summary>
    ///     This attribute is used for comparing 2 int typed attributes.
    /// </summary>
    public class CompareLongAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Initialize an instance of CompareLongAttribute class.
        /// </summary>
        /// <param name="milestone"></param>
        public CompareLongAttribute(long milestone)
        {
            Milestone = milestone;
        }

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
        public int Comparision { get; set; } = 0;

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
                case -2:
                {
                    if (convertedValue >= Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));

                    break;
                }
                case -1:
                {
                    if (convertedValue > Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));
                    break;
                }
                case 0: // Value must be equal to milestone.
                {
                    if (convertedValue != Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));
                    break;
                }
                case 1: // Value must be larger than or equal to milestone.
                {
                    if (convertedValue < Milestone)
                        return
                            new ValidationResult(string.Format(FormatErrorMessage(validationContext.DisplayName),
                                Milestone));
                    break;
                }
                case 2: // Value must be larger than milestone.
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
    }
}