using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of RequiredIfAttribute with given information.
        /// </summary>
        /// <param name="dependentProperty"></param>
        /// <param name="targetValue"></param>
        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Check whether property is valid or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            // get a reference to the property this validation depends upon
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(_dependentProperty);

            if (field != null)
            {
                // get the value of the dependent property
                var dependentvalue = field.GetValue(
                    validationContext.ObjectInstance, null);

                // compare the value against the target value
                if ((dependentvalue == null &&
                     _targetValue == null) ||
                    (dependentvalue != null &&
                     dependentvalue.Equals(_targetValue)))
                {
                    // match => means we should try validating this field
                    if (!_innerAttribute.IsValid(value))
                        // validation failed - return an error
                        return new ValidationResult(ErrorMessage,
                            new[] {validationContext.MemberName});
                }
            }

            return ValidationResult.Success;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Initialize an instance of required attribute to validate require section.
        /// </summary>
        private readonly RequiredAttribute _innerAttribute = new RequiredAttribute();

        /// <summary>
        ///     The property which is used for comparision.
        /// </summary>
        private readonly string _dependentProperty;

        /// <summary>
        ///     The value which dependent property must match to execute attribute.
        /// </summary>
        private readonly object _targetValue;

        #endregion
    }
}