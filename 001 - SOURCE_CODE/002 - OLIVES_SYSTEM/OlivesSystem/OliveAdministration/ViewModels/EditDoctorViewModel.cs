using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Models;
using Shared.ViewModels;

namespace DotnetSignalR.ViewModels
{
    public class EditDoctorViewModel : EditPersonViewModel
    {
        [CoordinateValidation(ErrorMessage = ErrorCodes.InvalidAddress)]
        public Coordinate Address { get; set; }

        [Required(ErrorMessage = ErrorCodes.SpecializationIsRequired)]
        [MaxLength(FieldLength.SpecializationMaxLength, ErrorMessage = ErrorCodes.InvalidSpecializationLength)]
        public string Specialization { get; set; }
        
        public double Rank { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = ErrorCodes.IdentityIsRequired)]
        [MaxLength(FieldLength.IdentityCardMaxLength, ErrorMessage = ErrorCodes.InvalidIdentityCardLength)]
        public string IdentityCardNo { get; set; }
    }
}