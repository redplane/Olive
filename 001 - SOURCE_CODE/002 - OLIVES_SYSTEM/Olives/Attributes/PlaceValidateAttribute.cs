using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Shared.Interfaces;

namespace Olives.Attributes
{
    public class PlaceValidateAttribute : ValidationAttribute
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
            // Retrieve the repository place from Dependency Resolver.
            var repositoryPlace = DependencyResolver.Current.GetService<IRepositoryPlace>();

            // Invalid value.
            if (value == null)
                return ValidationResult.Success;

            // Value is not an integer.
            if (!(value is int))
                throw new Exception("Value must be an instance of integer.");

            // Medical record id.
            var placeId = Convert.ToInt32(value);

            // Find the medical inside the repository.
            var place = repositoryPlace.FindPlace(placeId, City, CityNameComparision, Country,
                CountryNameComparision);

            if (place == null)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        #endregion

        #region Property

        /// <summary>
        ///     Name of city.
        /// </summary>
        public string City { get; set; } = null;

        /// <summary>
        ///     Name of city comparision mode.
        /// </summary>
        public StringComparison? CityNameComparision { get; set; }

        /// <summary>
        ///     Name of country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        ///     Comparision mode of medical record.
        /// </summary>
        public StringComparison? CountryNameComparision;

        #endregion
    }
}