using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterPersonViewModel
    {
        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        [RegexMatch(Regexes.EmailFilter, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        public string Email { get; set; }

        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidPhone")]
        public string Phone { get; set; }

        /// <summary>
        ///     Time after which account was modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public long? MinLastModified { get; set; }

        /// <summary>
        ///     Time before which account had been modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public long? MaxLastModified { get; set; }

        /// <summary>
        ///     Person first name.
        /// </summary>
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidLastName")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Person last name.
        /// </summary>
        [MaxLength(FieldLength.FirstNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidFirstName")]
        public string LastName { get; set; }

        /// <summary>
        ///     Date after that person was born
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxBirthday", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public long? MinBirthday { get; set; }

        /// <summary>
        ///     Date before which that person had been born
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinBirthday", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public long? MaxBirthday { get; set; }

        /// <summary>
        ///     Gender of person
        /// </summary>
        [InEnumerationsArray(new object[] {Enumerations.Gender.Female, Enumerations.Gender.Male},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidGender")]
        public int? Gender { get; set; }

        /// <summary>
        ///     Amount of money user's must be higher than.
        /// </summary>
        [NumericPropertyCompare("MinMoney", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinMoney { get; set; }

        /// <summary>
        ///     Amount of money user's must be lower than.
        /// </summary>
        [NumericPropertyCompare("MinMoney", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxMoney { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public long? MinCreated { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public long? MaxCreated { get; set; }

        /// <summary>
        ///     Status of account [0 - Disabled | 1 - Pending | 2 - Active].
        /// </summary>
        [InAccountStatus(new[] {StatusAccount.Active, StatusAccount.Inactive, StatusAccount.Pending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidAccountStatus")]
        public int? Status { get; set; }

        [InEnumerationsArray(new object[] {Enumerations.Role.Admin, Enumerations.Role.Doctor, Enumerations.Role.Patient},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidRole")]
        public int? Role { get; set; }
    }
}