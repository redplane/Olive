using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Models;

namespace Shared.ViewModels
{
    public class EditPersonViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrorCodes.DoctorIdIsRequired)]
        public string Id { get; set; }

        [Required(ErrorMessage = ErrorCodes.FirstNameIsRequired)]
        [MaxLength(32, ErrorMessage = ErrorCodes.InvalidFirstNameLength)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ErrorCodes.LastNameIsRequired)]
        [MaxLength(32, ErrorMessage = ErrorCodes.InvalidLastNameLength)]
        public string LastName { get; set; }

        [Required(ErrorMessage = ErrorCodes.BirthdayIsRequired)]
        [CompareLong(Times.MinimumSelectionTime, Comparision = -1, ErrorMessageEqualSmaller = ErrorCodes.InvalidBirthday )]
        public long? Birthday { get; set; }

        [Range(Constants.Gender.Male, Constants.Gender.Female, ErrorMessage = ErrorCodes.InvalidGender)]
        public byte Gender { get; set; }
        
        [MaxLength(FieldLength.PasswordMaxLength, ErrorMessage = ErrorCodes.InvalidPasswordLength)]
        [RegularExpression(Regexes.Password, ErrorMessage = ErrorCodes.InvalidPasswordFormat)]
        public string Password { get; set; }

        [Required(ErrorMessage = ErrorCodes.PhoneNumberIsRequired)]
        [RegularExpression(Regexes.Phone, ErrorMessage = ErrorCodes.InvalidPhoneFormat)]
        public string Phone { get; set; }

        public double Money { get; set; }

        [Range(AccountStatus.Inactive, AccountStatus.Active, ErrorMessage = ErrorCodes.InvalidAccountStatus)]
        public byte Status { get; set; }
    }
}