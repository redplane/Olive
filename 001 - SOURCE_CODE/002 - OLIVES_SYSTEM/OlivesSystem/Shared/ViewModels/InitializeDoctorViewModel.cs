using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Models;

namespace Shared.ViewModels
{
    public class InitializeDoctorViewModel : InitializePersonViewModel
    {
        /// <summary>
        ///     For doctor, address is compulsory.
        /// </summary>
        [Required(ErrorMessage = ErrorCodes.AddressIsRequired)]
        [CoordinateValidation(ErrorMessage = ErrorCodes.InvalidAddress)]
        public Coordinate Address { get; set; }

        [Required(ErrorMessage = ErrorCodes.SpecializationIsRequired)]
        [MaxLength(FieldLength.SpecializationMaxLength, ErrorMessage = ErrorCodes.InvalidSpecializationLength)]
        public string Specialization { get; set; }
    }
}