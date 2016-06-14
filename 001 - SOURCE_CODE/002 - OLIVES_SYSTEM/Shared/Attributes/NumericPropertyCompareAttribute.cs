using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Shared.Enumerations;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "This attribute is designed to be a base class for other attributes.")]
    public class NumericPropertyCompareAttribute : ValidationAttribute
    {
        #region Constructor

        public NumericPropertyCompareAttribute(string model)
        {
            // Invalid property name.
            if (model == null)
                throw new ArgumentNullException("model");

            // Update property name.
            Model = model;
        }

        #endregion

        /// <summary>
        ///     Property which is used for comparing with the source one.
        /// </summary>
        public string Model { get; }

        /// <summary>
        ///     Model display name.
        /// </summary>
        public string ModelDisplayName { get; internal set; }
        
        /// <summary>
        /// Which way should 2 models be compared with each other.
        /// </summary>
        public Comparision Comparision { get; set; } = Comparision.Equal;

        /// <summary>
        /// Dunno what this means.
        /// TODO: Update documentation later.
        /// </summary>
        public override bool RequiresValidationContext
        {
            get { return true; }
        }

        /// <summary>
        ///     Override the base function to display error message with custom parameters.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, ModelDisplayName ?? Model);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            #region Original model

            // Value hasn't been defined.
            if (value == null)
                return ValidationResult.Success;
            
            // Retrieve model information.
            var modelInfo = validationContext.ObjectType.GetProperty(Model);

            // Target model hasn't been initialized.
            if (modelInfo == null)
                return ValidationResult.Success;

            #endregion

            #region Compared property

            // Retrieve value from target model.
            var modelValue = modelInfo.GetValue(validationContext.ObjectInstance, null);
            
            if (modelValue == null)
                return ValidationResult.Success;

            #endregion

            // Convert both value to long.
            var compareOrigin = Convert.ToInt64(value);
            var milestone = Convert.ToInt64(modelValue);

            switch (Comparision)
            {
                case Comparision.Lower:
                    {
                        if (compareOrigin >= milestone)
                        {
                            if (ModelDisplayName == null)
                                ModelDisplayName = RetrievePropertyDisplayName(validationContext.ObjectType, Model);

                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        break;
                    }
                case Comparision.LowerEqual:
                    {
                        if (compareOrigin > milestone)
                        {
                            if (ModelDisplayName == null)
                                ModelDisplayName = RetrievePropertyDisplayName(validationContext.ObjectType, Model);

                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        break;
                    }
                case Comparision.Equal: // Value must be equal to milestone.
                    {
                        if (compareOrigin != milestone)
                        {
                            if (ModelDisplayName == null)
                                ModelDisplayName = RetrievePropertyDisplayName(validationContext.ObjectType, Model);

                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        break;
                    }
                case Comparision.GreaterEqual: // Value must be larger than or equal to milestone.
                    {
                        if (compareOrigin < milestone)
                        {
                            if (ModelDisplayName == null)
                                ModelDisplayName = RetrievePropertyDisplayName(validationContext.ObjectType, Model);

                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        break;
                    }
                case Comparision.Greater: // Value must be larger than milestone.
                    {
                        if (compareOrigin <= milestone)
                        {
                            if (ModelDisplayName == null)
                                ModelDisplayName = RetrievePropertyDisplayName(validationContext.ObjectType, Model);

                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Retrieve display name of a property
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private string RetrievePropertyDisplayName(Type containerType, string propertyName)
        {
            var typeDescriptor = GetTypeDescriptor(containerType);
            var property = typeDescriptor.GetProperties().Find(propertyName, true);
            if (property == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "The property {0}.{1} could not be found", containerType.FullName, propertyName));
            }
            var attributes = property.Attributes.Cast<Attribute>();
            var enumerable = attributes as Attribute[] ?? attributes.ToArray();
            var display = enumerable.OfType<DisplayAttribute>().FirstOrDefault();
            if (display != null)
                return display.GetName();

            var displayName = enumerable.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayName != null)
                return displayName.DisplayName;

            return propertyName;
        }

        private static ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }
    }
}