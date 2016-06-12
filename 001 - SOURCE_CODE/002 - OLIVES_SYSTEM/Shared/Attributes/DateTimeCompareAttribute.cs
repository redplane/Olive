using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Shared.Enumerations;

namespace Shared.Attributes
{
    /// <summary>
    ///     Validation attribute to validate two <see cref="DateTime" /> values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class DateTimeCompareAttribute : PropertyValidationAttribute
    {
        #region Constructor 

        /// <summary>
        ///     Initializes new instance of the <see cref="DateTimeCompareAttribute" /> class.
        /// </summary>
        /// <param name="otherProperty">The name of the other property.</param>
        /// <param name="comparison"></param>
        public DateTimeCompareAttribute(string otherProperty, Comparision comparison)
            : base(otherProperty)
        {
            if (!Enum.IsDefined(typeof (Comparision), comparison))
                throw new ArgumentException("Undefined value", "comparison");

            _comparedProperty = otherProperty;
            _comparison = comparison;
            
        }

        #endregion

        #region Overrides 

        /// <summary>
        /// Override IsValid function to check whether the comparision is valid or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Model is undefined.
            if (value == null)
                return ValidationResult.Success;

            #region First element
            
            // Retrieve date time instance of object.
            var date = GetDate(value);

            // First element hasn't been initialized.
            if (!date.HasValue)
                return ValidationResult.Success;

            #endregion

            #region Compared element

            // Retrieve the value of compared element.
            var comparedElement = GetValue(validationContext);

            // Compared element hasn't been specified.
            if (comparedElement == null)
                return ValidationResult.Success;

            // Retrieve date which is used for comparing with source.
            var comparedDate = GetDate(comparedElement);

            // Compared date hasn't been specified.
            if (comparedDate == null)
                return ValidationResult.Success;

            // Do comparision, if result is false, it means the comparision is failed.
            if (!IsValidComparision(date.Value, comparedDate.Value))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            #endregion

            return IsValid(date.Value, validationContext);
        }

        /// <summary>
        /// Override this function to support muli-parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _comparedProperty);
        }

        #endregion

        #region Fields 

        /// <summary>
        /// Which comparision the attribute should do to compare 2 properties.
        /// </summary>
        private readonly Comparision _comparison;

        /// <summary>
        /// Property name which is used for comparing with the source one.
        /// </summary>
        private readonly string _comparedProperty;

        #endregion

        #region Methods 

        /// <summary>
        ///     Retrieve DateTime instance from an object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private DateTime? GetDate(object value)
        {
            return (DateTime?) value;
        }
        
        /// <summary>
        ///     Check whether the comparision is valid or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        private bool IsValidComparision(DateTime value, DateTime otherValue)
        {
            switch (_comparison)
            {
                case Comparision.Equal:
                    if (value != otherValue)
                    {
                        return false;
                    }
                    break;
                case Comparision.NotEqual:
                    if (value == otherValue)
                    {
                        return false;
                    }
                    break;
                case Comparision.Greater:
                    if (value <= otherValue)
                    {
                        return false;
                    }
                    break;
                case Comparision.GreaterEqual:
                    if (value < otherValue)
                    {
                        return false;
                    }
                    break;
                case Comparision.Lower:
                    if (value >= otherValue)
                    {
                        return false;
                    }
                    break;
                case Comparision.LowerEqual:
                    if (value > otherValue)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        #endregion
    }
}