using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Initialize
{
    public class InitializePlaceViewModel
    {
        /// <summary>
        ///     City name.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(FieldLength.CityNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string City { get; set; }

        /// <summary>
        ///     Name of country where this city belongs to.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(FieldLength.CityNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Country { get; set; }
    }
}