using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace Shared.ViewModels
{
    public class CreateDoctorViewModel : CreatePersonViewModel
    {
        [Required(ErrorMessage = ErrorCodes.SpecializationIsRequired)]
        [MaxLength(FieldLength.SpecializationMaxLength, ErrorMessage = ErrorCodes.InvalidSpecializationLength)]
        public string Specialization { get; set; }

        [Required(ErrorMessage = ErrorCodes.SpecializationAreasIsRequired)]
        public string[] SpecializationAreas { get; set; }
    }
}