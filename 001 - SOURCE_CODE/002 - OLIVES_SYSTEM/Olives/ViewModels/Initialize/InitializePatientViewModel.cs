﻿using System.ComponentModel.DataAnnotations;
using Olives.Attributes;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializePatientViewModel
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
        [InEnumerationsArray(new object[] {Gender.Female, Gender.Male},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidGender")]
        public Gender Gender { get; set; } = Gender.Male;

        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireEmail")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        [RegularExpression(Regexes.Email, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        [AccountValidate(AccountValidateInputType.Email, false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueHasBeenRegistered")]
        public string Email { get; set; }

        /// <summary>
        ///     Password of this account.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequirePassword")]
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
    }
}