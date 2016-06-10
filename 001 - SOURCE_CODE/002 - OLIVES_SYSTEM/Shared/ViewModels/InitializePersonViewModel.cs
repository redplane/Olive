using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class InitializePersonViewModel
    {
        /// <summary>
        ///     Person first name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "RequireFirstName")]
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidFirstName")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Person last name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "RequireLastName")]
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidLastName")]
        public string LastName { get; set; }
        
        /// <summary>
        ///     Birthday (ticks).
        /// </summary>
        [TickToYearCompare(Values.MinimumAllowedYear, Comparision = 1, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidBirthyear")]
        public long? Birthday { get; set; }

        /// <summary>
        ///     Person gender.
        /// </summary>
        [Range(Constants.Gender.Male, Constants.Gender.Female, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidGender")]
        public byte Gender { get; set; }

        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "RequireEmail")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        [RegularExpression(Regexes.Email, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        public string Email { get; set; }

        /// <summary>
        ///     Password of this account.
        /// </summary>
        [MinLength(Values.MinPasswordLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "MinLengthPassword")]
        [MaxLength(Values.MaxPasswordLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "MaxLengthPassword")]
        [RegexMatch(Regexes.Password, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "RegexPassword")]
        public string Password { get; set; }

        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPhone")]
        
        public string Phone { get; set; }
    }
}