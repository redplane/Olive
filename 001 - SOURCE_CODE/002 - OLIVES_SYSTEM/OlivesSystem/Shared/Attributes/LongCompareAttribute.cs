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
        ///     Message which will be thrown when value must be equal to the compared one.
        /// </summary>
        public string ErrorMessageEqual = "";

        /// <summary>
        ///     Message which will be thrown when value must be larger than or equal to the compared one.
        /// </summary>
        public string ErrorMessageEqualHigher = "";

        /// <summary>
        ///     Message which will be thrown when value must be smaller than or equal to the compared one.
        /// </summary>
        public string ErrorMessageEqualSmaller = "";

        /// <summary>
        ///     Message which will be thrown when value must be higher than the compared one.
        /// </summary>
        public string ErrorMessageHigher = "";

        /// <summary>
        ///     Message which will be thrown when value must be equal lower than the compared one.
        /// </summary>
        public string ErrorMessageLower = "";

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
                    {
                        if (string.IsNullOrEmpty(ErrorMessageLower))
                            return
                                new ValidationResult(
                                    $"{validationContext.DisplayName} should be smaller than {Milestone}");

                        return
                            new ValidationResult(string.Format(ErrorMessageLower, validationContext.DisplayName,
                                Milestone));
                    }
                    break;
                }
                case -1:
                {
                    if (convertedValue > Milestone)
                    {
                        if (string.IsNullOrEmpty(ErrorMessageEqualSmaller))
                            return
                                new ValidationResult(
                                    $"{validationContext.DisplayName} should be smaller than or equal to {Milestone}");

                        return
                            new ValidationResult(string.Format(ErrorMessageEqualSmaller, validationContext.DisplayName,
                                Milestone));
                    }
                    break;
                }
                case 0: // Value must be equal to milestone.
                {
                    if (convertedValue != Milestone)
                    {
                        if (string.IsNullOrEmpty(ErrorMessageEqual))
                            return
                                new ValidationResult($"{validationContext.DisplayName} should be equal to {Milestone}");

                        return
                            new ValidationResult(string.Format(ErrorMessageEqual, validationContext.DisplayName,
                                Milestone));
                    }
                    break;
                }
                case 1: // Value must be larger than or equal to milestone.
                {
                    if (convertedValue < Milestone)
                    {
                        if (string.IsNullOrEmpty(ErrorMessageEqualHigher))
                            return
                                new ValidationResult(
                                    $"{validationContext.DisplayName} should be larger than or equal to {Milestone}");

                        return
                            new ValidationResult(string.Format(ErrorMessageEqualHigher, validationContext.DisplayName,
                                Milestone));
                    }
                    break;
                }
                case 2: // Value must be larger than milestone.
                {
                    if (convertedValue <= Milestone)
                    {
                        if (string.IsNullOrEmpty(ErrorMessageHigher))
                            return
                                new ValidationResult(
                                    $"{validationContext.DisplayName} should be larger than {Milestone}");

                        return
                            new ValidationResult(string.Format(ErrorMessageHigher, validationContext.DisplayName,
                                Milestone));
                    }
                    break;
                }
                default:
                    throw new NotImplementedException();
            }

            return ValidationResult.Success;
        }
    }
}