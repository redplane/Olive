using System.ComponentModel.DataAnnotations;
using Olives.Enumerations.Filter;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace Olives.ViewModels.Filter
{
    public class FilterPatientViewModel : IPagination
    {
        public int? Id { get; set; }

        /// <summary>
        /// Person who sent the patient filter request.
        /// </summary>
        public int Requester { get; set; }

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

        /// <summary>
        /// Whether records should be sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] {SortDirection.Ascending, SortDirection.Decending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public SortDirection Direction { get; set; } = SortDirection.Ascending;

        /// <summary>
        /// Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(
            new object[]
            {
                PatientFilterSort.Email, PatientFilterSort.Phone, PatientFilterSort.FirstName, PatientFilterSort.LastName, PatientFilterSort.Birthday,
                PatientFilterSort.Gender
            }, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public PatientFilterSort Sort { get; set; } = PatientFilterSort.Email;

        /// <summary>
        /// Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = FieldLength.PageIndexMin;

        /// <summary>
        /// Records per page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}