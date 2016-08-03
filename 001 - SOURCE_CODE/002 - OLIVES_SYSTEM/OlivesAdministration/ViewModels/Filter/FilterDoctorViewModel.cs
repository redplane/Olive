using System.ComponentModel.DataAnnotations;
using OlivesAdministration.Enumerations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Filter
{
    public class FilterDoctorViewModel : IPagination
    {
        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        [RegexMatch(Regexes.EmailFilter, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        public string Email { get; set; }

        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPhone")]
        public string Phone { get; set; }

        /// <summary>
        ///     Time after which account was modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public long? MinLastModified { get; set; }

        /// <summary>
        ///     Time before which account had been modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public long? MaxLastModified { get; set; }

        /// <summary>
        ///     Person name.
        /// </summary>
        [MaxLength(FieldLength.FullNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        /// <summary>
        ///     Date after that person was born
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxBirthday", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public long? MinBirthday { get; set; }

        /// <summary>
        ///     Date before which that person had been born
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinBirthday", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public long? MaxBirthday { get; set; }

        /// <summary>
        ///     Gender of person
        /// </summary>
        [InEnumerationsArray(new object[] { Shared.Enumerations.Gender.Male, Shared.Enumerations.Gender.Female },
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidGender")]
        public int? Gender { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public long? MinCreated { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public long? MaxCreated { get; set; }

        /// <summary>
        ///     Amount of money user's must be higher than.
        /// </summary>
        [NumericPropertyCompare("MaxMoney", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinMoney { get; set; }

        /// <summary>
        ///     Amount of money user's must be lower than.
        /// </summary>
        [NumericPropertyCompare("MinMoney", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxMoney { get; set; }

        /// <summary>
        ///     Name of city where doctor lives.
        /// </summary>
        [StringLength(FieldLength.CityNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string City { get; set; }

        /// <summary>
        ///     Name of country where doctor lives.
        /// </summary>
        [StringLength(FieldLength.CountryNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Country { get; set; }

        /// <summary>
        ///     Id of doctor's specialty.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]

        public int? Specialty { get; set; }

        [Range(Values.MinDoctorRank, Values.MaxDoctorRank, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MaxRank", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinRank { get; set; }

        [Range(Values.MinDoctorRank, Values.MaxDoctorRank, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MinRank", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxRank { get; set; }

        /// <summary>
        /// Whether records should be sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] {SortDirection.Ascending, SortDirection.Decending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public SortDirection Direction { get; set; } = SortDirection.Ascending;

        /// <summary>
        /// Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(new object[] { FilterDoctorSort.Created, FilterDoctorSort.Birthday, FilterDoctorSort.FirstName, FilterDoctorSort.Gender, FilterDoctorSort.LastModified, FilterDoctorSort.LastName, FilterDoctorSort.Money, FilterDoctorSort.Status }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public FilterDoctorSort Sort { get; set; } = FilterDoctorSort.FirstName;

        /// <summary>
        /// Status of account.
        /// </summary>
        public StatusAccount? Status { get; set; }

        /// <summary>
        /// Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = FieldLength.PageIndexMin;

        /// <summary>
        /// Number of records per page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}