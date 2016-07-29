using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Shared.Enumerations;
using Shared.Interfaces;

namespace Shared.Attributes
{
    public class EpochTimeCompareAttribute : ValidationAttribute
    {
        #region Properties

        /// <summary>
        ///     Date which is needed to be compared.
        /// </summary>
        private readonly double? _milliseconds;

        /// <summary>
        ///     Date which is similar to millisecond.
        /// </summary>
        private readonly DateTime _date;

        /// <summary>
        ///     Comparision mode.
        /// </summary>
        public Comparision Comparision { get; set; } = Comparision.Equal;
        
        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of EpochTimeCompareAttribute with given information.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="millisecond"></param>
        public EpochTimeCompareAttribute(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            // Initialize a date from the given information.
            _date = new DateTime(year, month, day, hour, minute, second, millisecond);

            // Calculate the approx millisecond.
            _milliseconds =
                _date.ToUniversalTime()
                    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    .TotalMilliseconds;
            
        }

        /// <summary>
        ///     Initialize an instance of EpochTimeCompareAttribute with given information.
        /// </summary>
        /// <param name="date"></param>
        public EpochTimeCompareAttribute(DateTime date)
        {
            try
            {
                _date = date;
                _milliseconds =
                    _date.ToUniversalTime()
                        .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                        .TotalMilliseconds;
            }
            catch (Exception)
            {
                _milliseconds = null;
            }
        }

        /// <summary>
        ///     Initialize an instance of EpochTimeCompareAttribute with given information.
        /// </summary>
        /// <param name="year"></param>
        public EpochTimeCompareAttribute(int year)
        {
            _date = new DateTime(year, 12, 31, 23, 59, 59, 999);
            _milliseconds = _date.ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Check whether the validation is valid or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Value hasn't been defined. Treat this validation be successful.
            if (value == null)
                return ValidationResult.Success;

            if (_milliseconds == null)
                return ValidationResult.Success;

            // Every input will be casted to milliseconds.
            double milliseconds;

            // Cast value to date time.
            if (value is DateTime)
            {
                var castedDate = (DateTime) value;
                milliseconds =
                    castedDate.ToUniversalTime()
                        .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                        .TotalMilliseconds;
            }
            else
                milliseconds = Convert.ToDouble(value);

            #region Comparision doing

            switch (Comparision)
            {
                case Comparision.Lower:
                    if (milliseconds >= _milliseconds.Value)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    break;
                case Comparision.LowerEqual:
                    if (milliseconds > _milliseconds.Value)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    break;
                case Comparision.Equal:
                    if (milliseconds != _milliseconds)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    break;
                case Comparision.NotEqual:
                    if (milliseconds == _milliseconds)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    break;
                case Comparision.GreaterEqual:
                    if (milliseconds < _milliseconds.Value)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    break;
                case Comparision.Greater:
                    if (milliseconds <= _milliseconds.Value)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    break;
                default:
                    throw new Exception("No comparision mode has been initialized");
            }

            return ValidationResult.Success;

            #endregion
        }

        /// <summary>
        ///     Override format error message to support multi parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _date.Year, _date.Month,
                _date.Day);
        }

        #endregion
    }
}