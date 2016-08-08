using System.ComponentModel.DataAnnotations;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Initialize
{
    public class InitializeMedicalCategoryViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }
    }
}