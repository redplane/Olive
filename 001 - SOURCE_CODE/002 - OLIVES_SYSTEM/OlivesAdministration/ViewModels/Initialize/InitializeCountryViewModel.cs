using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Initialize
{
    public class InitializeCountryViewModel
    {
        [StringLength(FieldLength.CountryNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }
    }
}