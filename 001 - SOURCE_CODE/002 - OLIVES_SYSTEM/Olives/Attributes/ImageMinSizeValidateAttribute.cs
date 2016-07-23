using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;

namespace Olives.Attributes
{
    public class ImageMinSizeValidateAttribute : ValidationAttribute
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
            if (MinWidth != null && image.Width < MinWidth.Value)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            // Maximum height is defined.
            if (MinHeight != null && image.Height > MinHeight.Value)
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
            if (MinWidth != null && MinHeight != null)
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinWidth, MinHeight);

            // Only width is defined.
            if (MinWidth != null)
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinWidth);

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinHeight);
        }

        #endregion

        #region Property

        /// <summary>
        /// Maximum width of image.
        /// </summary>
        public int? MinWidth { get; set; }

        /// <summary>
        /// Maximum height of image.
        /// </summary>
        public int? MinHeight { get; set; }
        
        #endregion
    }
}