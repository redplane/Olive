using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Edit
{
    public class EditPlaceViewModel
    {
        /// <summary>
        ///     City name.
        /// </summary>
        [StringLength(FieldLength.CityNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string City { get; set; }

        /// <summary>
        ///     Name of country where this city belongs to.
        /// </summary>
        [StringLength(FieldLength.CityNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Country { get; set; }
    }
}