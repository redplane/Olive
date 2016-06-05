using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Shared.Models;

namespace Shared.Attributes
{
    /// <summary>
    ///     This attribute is for validating Geometry coordinate.
    /// </summary>
    public class CoordinateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Value is not specified.
            if (value == null)
                return ValidationResult.Success;

            if (!(value is Coordinate))
                return new ValidationResult(string.Format("{0} is not coordinate", validationContext.DisplayName));

            // Cast value to Coordinate instance.
            var coordinate = (Coordinate) value;
            
            // Invalid latitude.
            if (coordinate.Latitude < -90 || coordinate.Latitude > 90)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            // Invalid longitude
            if (coordinate.Longitude < -90 || coordinate.Longitude > 90)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}