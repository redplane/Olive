﻿using System.ComponentModel.DataAnnotations;
using OliveAdmin.Enumerations;
using OliveAdmin.Enumerations.Filter;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Filter
{
    public class FilterPatientViewModel : IPagination
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
        ///     Text comparision mode of email.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision EmailComparision { get; set; }

        /// <summary>
        ///     First name of patient.
        /// </summary>
        [StringLength(FieldLength.FirstNameMaxLength, ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Comparision mode of first name.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision FirstNameComparision { get; set; }

        /// <summary>
        ///     Last name of patient.
        /// </summary>
        [StringLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string LastName { get; set; }

        /// <summary>
        ///     Comparision mode of last name.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision LastNameComparision { get; set; }

        /// <summary>
        ///     Date after that person was born
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxBirthday", Comparision = Comparision.LowerEqual,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinBirthday { get; set; }

        /// <summary>
        ///     Date before which that person had been born
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinBirthday", Comparision = Comparision.GreaterEqual,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxBirthday { get; set; }

        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "InvalidPhone")]
        public string Phone { get; set; }

        /// <summary>
        ///     Phone number text comparision mode.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision PhoneComparision { get; set; }

        /// <summary>
        ///     Available genders of people.
        /// </summary>
        public Gender[] Genders { get; set; }

        /// <summary>
        ///     People available statuses.
        /// </summary>
        public AccountStatus[] Statuses { get; set; }

        /// <summary>
        ///     Time after which account was created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinCreated { get; set; }

        /// <summary>
        ///     Time before which account had been created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxCreated { get; set; }

        /// <summary>
        ///     Time after which account was modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinLastModified { get; set; }

        /// <summary>
        ///     Time before which account had been modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual,
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxLastModified { get; set; }

        /// <summary>
        ///     Whether record should be filtered ascending or decending.
        /// </summary>
        [InEnumerationsArray(new object[] { SortDirection.Ascending, SortDirection.Decending },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(new object[] { FilterPatientSort.Gender, FilterPatientSort.Birthday, FilterPatientSort.Created, FilterPatientSort.Created, FilterPatientSort.FirstName, FilterPatientSort.LastModified, FilterPatientSort.LastName, FilterPatientSort.Status }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public FilterPatientSort SortProperty { get; set; } = FilterPatientSort.FirstName;

        /// <summary>
        ///     Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
             ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; }

        /// <summary>
        ///     Number of records should be displayed on a page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}