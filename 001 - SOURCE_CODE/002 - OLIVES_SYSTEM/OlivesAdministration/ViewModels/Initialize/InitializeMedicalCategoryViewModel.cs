using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Initialize
{
    public class InitializeMedicalCategoryViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(Values.MaxMedicalCategoryNameLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }
    }
}