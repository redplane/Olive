using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace Shared.Attributes
{
    /// <summary>
    ///     This attribute is for validating Geometry coordinate.
    /// </summary>
    public class CoordinateValidateAttribute : ValidationAttribute
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
            if (coordinate.Latitude < -85 || coordinate.Latitude > 85)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            // Invalid longitude
            if (coordinate.Longitude < -180 || coordinate.Longitude > 180)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}