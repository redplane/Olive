using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Shared.Interfaces;

namespace Olives.Attributes
{
    public class MedicalCategoryValidateAttribute : ValidationAttribute
    {
        #region Property
        
        /// <summary>
        /// Name of medical record.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// Comparision mode of medical record.
        /// </summary>
        public StringComparison? Comparision;

        #endregion
        
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
            
            // Value is not an integer.
            if (!(value is int))
                throw new Exception("Value must be an instance of integer.");

            // Medical record id.
            var medicalRecord = Convert.ToInt32(value);

            // Find an instance of IRepositoryMedical in Dependency Resolver.
            var repositoryMedical = DependencyResolver.Current.GetService<IRepositoryMedical>();

            // Find the medical inside the repository.
            var result = repositoryMedical.FindMedicalCategoryAsync(medicalRecord, Name, Comparision).Result;

            if (result == null)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        #endregion
        
    }
}