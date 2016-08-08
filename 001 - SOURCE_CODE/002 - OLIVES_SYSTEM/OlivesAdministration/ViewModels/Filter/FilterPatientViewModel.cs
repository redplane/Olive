using System.ComponentModel.DataAnnotations;
using OlivesAdministration.Enumerations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Filter
{
    public class FilterPatientViewModel : IPagination
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
        ///     Person name.
        /// </summary>
        [MaxLength(FieldLength.FullNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

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
        [InEnumerationsArray(new object[] {Shared.Enumerations.Gender.Female, Shared.Enumerations.Gender.Male},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidGender")]
        public int? Gender { get; set; }

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

        [NumericPropertyCompare("MaxWeight", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MinWeight { get; set; }

        [NumericPropertyCompare("MinWeight", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MaxWeight { get; set; }

        [NumericPropertyCompare("MaxHeight", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MinHeight { get; set; }

        [NumericPropertyCompare("MinHeight", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MaxHeight { get; set; }

        /// <summary>
        ///     Current status of patient account.
        /// </summary>
        [InEnumerationsArray(new object[] {StatusAccount.Active, StatusAccount.Inactive, StatusAccount.Pending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public int? Status { get; set; }

        /// <summary>
        ///     Whether record should be filtered ascending or decending.
        /// </summary>
        public SortDirection Direction { get; set; } = SortDirection.Ascending;

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        public FilterPatientSort Sort { get; set; } = FilterPatientSort.FirstName;

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}