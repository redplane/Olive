using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Models;

namespace Shared.ViewModels
{
    public class CreatePersonViewModel
    {
        [Required(ErrorMessage = ErrorCodes.FirstNameIsRequired)]
        [MaxLength(32, ErrorMessage = ErrorCodes.InvalidFirstNameLength)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ErrorCodes.LastNameIsRequired)]
        [MaxLength(32, ErrorMessage = ErrorCodes.InvalidLastNameLength)]
        public string LastName { get; set; }

        public long Birthday { get; set; }

        [Range(Constants.Gender.Male, Constants.Gender.Female, ErrorMessage = ErrorCodes.InvalidGender)]
        public byte Gender { get; set; }

        public Coordinate Address { get; set; }

        [Required(ErrorMessage = ErrorCodes.EmailIsRequired)]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessage = ErrorCodes.InvalidEmailLength)]
        [RegularExpression(Regexes.Email, ErrorMessage = ErrorCodes.InvalidEmail)]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrorCodes.PasswordIsRequired)]
        [MaxLength(FieldLength.PasswordMaxLength, ErrorMessage = ErrorCodes.InvalidPasswordLength)]
        [RegularExpression(Regexes.Password, ErrorMessage = ErrorCodes.InvalidPasswordFormat)]
        public string Password { get; set; }

        [Required(ErrorMessage = ErrorCodes.PhoneNumberIsRequired)]
        [RegularExpression(Regexes.Phone, ErrorMessage = ErrorCodes.InvalidPhoneFormat)]
        public string Phone { get; set; }
    }
}