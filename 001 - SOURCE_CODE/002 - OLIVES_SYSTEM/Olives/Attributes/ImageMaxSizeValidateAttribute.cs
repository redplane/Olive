using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;

namespace Olives.Attributes
{
    public class ImageMaxSizeValidateAttribute : ValidationAttribute
    {
        #region Methods

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

            // Value is not an instance of Image.
            if (!(value is Image))
                throw new Exception("Value must be an instance of Image class.");

            // Cast value to image.
            var image = (Image) value;

            // Maximum width is defined.
            if (MaxWidth != null && image.Width > MaxWidth.Value)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            // Maximum height is defined.
            if (MaxHeight != null && image.Height > MaxHeight.Value)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        /// <summary>
        ///     Override this function to support muli-parameters and multilingual.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            // 2 dimensions are defined.
            if (MaxWidth != null && MaxHeight != null)
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxWidth, MaxHeight);

            // Only width is defined.
            if (MaxWidth != null)
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxWidth);

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxHeight);
        }

        #endregion

        #region Property

        /// <summary>
        ///     Maximum width of image.
        /// </summary>
        public int? MaxWidth { get; set; }

        /// <summary>
        ///     Maximum height of image.
        /// </summary>
        public int? MaxHeight { get; set; }

        #endregion
    }
}