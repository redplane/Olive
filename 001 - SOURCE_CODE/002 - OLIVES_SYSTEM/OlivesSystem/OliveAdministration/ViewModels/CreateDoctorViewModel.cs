using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Models;

namespace DotnetSignalR.ViewModels
{
    public class CreateDoctorViewModel
    {
        [Required(ErrorMessage = ErrorCodes.FirstNameIsRequired)]
        [MaxLength(32, ErrorMessage = ErrorCodes.InvalidFirstNameLength)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ErrorCodes.LastNameIsRequired)]
        [MaxLength(32, ErrorMessage = ErrorCodes.InvalidLastNameLength)]
        public string LastName { get; set; }

        public long Birthday { get; set; }

        [Range(Constants.Male, Constants.Female, ErrorMessage = ErrorCodes.InvalidGender)]
        public byte Gender { get; set; }

        public Coordinate Address { get; set; }

        [Required(ErrorMessage = ErrorCodes.EmailIsRequired)]
        [MaxLength(Constants.EmailMaxLength, ErrorMessage = ErrorCodes.InvalidEmailLength)]
        [RegularExpression(Regexes.Email, ErrorMessage = ErrorCodes.InvalidEmail)]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrorCodes.PasswordIsRequired)]
        [MaxLength(Constants.PasswordMaxLength, ErrorMessage = ErrorCodes.InvalidPasswordLength)]
        [RegularExpression(Regexes.Password, ErrorMessage = ErrorCodes.InvalidPasswordFormat)]
        public string Password { get; set; }

        [Required(ErrorMessage = ErrorCodes.PhoneNumberIsRequired)]
        [RegularExpression(Regexes.Phone, ErrorMessage = ErrorCodes.InvalidPhoneFormat)]
        public string Phone { get; set; }
        
        public double Money { get; set; }

        public long Created { get; set; }

        [Required(ErrorMessage = ErrorCodes.SpecializationIsRequired)]
        [MaxLength(Constants.SpecializationMaxLength, ErrorMessage = ErrorCodes.InvalidSpecializationLength)]
        public string Specialization { get; set; }

        [Required(ErrorMessage = ErrorCodes.SpecializationAreasIsRequired)]
        public string[] SpecializationAreas { get; set; }
    }
}