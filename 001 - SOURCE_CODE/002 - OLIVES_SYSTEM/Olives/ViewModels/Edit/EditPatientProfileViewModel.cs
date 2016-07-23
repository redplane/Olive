using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Edit
{
    public class EditPatientProfileViewModel
    {
        /// <summary>
        ///     Person first name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireFirstName")]
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidFirstName")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Person last name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireLastName")]
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidLastName")]
        public string LastName { get; set; }

        /// <summary>
        ///     Birthday (ticks).
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public long? Birthday { get; set; }

        /// <summary>
        ///     Person gender.
        /// </summary>
        [InEnumerationsArray(new object[] {Shared.Enumerations.Gender.Female, Shared.Enumerations.Gender.Male},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidGender")]
        public Gender? Gender { get; set; }

        /// <summary>
        ///     Password of this account.
        /// </summary>
        [MinLength(Values.MinPasswordLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "MinLengthPassword")]
        [MaxLength(Values.MaxPasswordLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "MaxLengthPassword")]
        [RegexMatch(Regexes.Password, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "RegexPassword")]
        public string Password { get; set; }


        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidPhone")]
        public string Phone { get; set; }

        [StringLength(FieldLength.MaxAddressLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Address { get; set; }

        /// <summary>
        ///     Weight of patient.
        /// </summary>
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double? Weight { get; set; }

        /// <summary>
        ///     Height of patient.
        /// </summary>
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double? Height { get; set; }
    }
}