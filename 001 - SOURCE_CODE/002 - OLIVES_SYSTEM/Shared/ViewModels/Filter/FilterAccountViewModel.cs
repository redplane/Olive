using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterAccountViewModel : IPagination
    {
        /// <summary>
        ///     Email of admin.
        /// </summary>
        [RegexMatch(Regexes.EmailFilter, ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "InvalidEmailFormat")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        public string Email { get; set; }

        /// <summary>
        ///     Email text comparision.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision EmailComparision { get; set; } = TextComparision.Contain;

        /// <summary>
        /// Account first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// First name comparision.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal, TextComparision.EqualIgnoreCase }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision FirstNameComparision { get; set; } = TextComparision.Contain;

        /// <summary>
        /// Account last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Last name comparision.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal, TextComparision.EqualIgnoreCase }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision LastNameComparision { get; set; } = TextComparision.Contain;

        /// <summary>
        ///     Password of admin account (encrypted)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Password comparision mode.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision PasswordComparision { get; set; } = TextComparision.Contain;

        /// <summary>
        ///     List of statuses.
        /// </summary>
        public AccountStatus[] Statuses { get; set; }

        /// <summary>
        ///     Collection of genders which can be displayed on result page.
        /// </summary>
        public Gender[] Genders { get; set; }

        /// <summary>
        ///     Account phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     Phone comparision mode.
        /// </summary>
        [InEnumerationsArray(new object[] { TextComparision.Contain, TextComparision.Equal },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public TextComparision PhoneComparision { get; set; }

        /// <summary>
        /// Minimum birthday.
        /// </summary>
        [NumericPropertyCompare("MaxBirthday", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinBirthday { get; set; }

        /// <summary>
        /// Maximum birthday.
        /// </summary>
        [NumericPropertyCompare("MinBirthday", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxBirthday { get; set; }

        /// <summary>
        ///     Collection of roles which can be displayed on result page.
        /// </summary>
        public Role[] Roles { get; set; }

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
        /// Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(new object[] { FilterAccountSort.Email, FilterAccountSort.FirstName, FilterAccountSort.LastName, FilterAccountSort.Status, FilterAccountSort.Gender, FilterAccountSort.Phone, FilterAccountSort.Birthday, FilterAccountSort.Role, FilterAccountSort.Created, FilterAccountSort.LastModified })]
        public FilterAccountSort SortProperty { get; set; }

        /// <summary>
        ///     Whether accounts should be sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] { SortDirection.Ascending, SortDirection.Decending },
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public SortDirection SortDirection { get; set; }

        /// <summary>
        ///     Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
             ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        /// <summary>
        ///     Number of records should be displayed on a page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
             ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; } = 0;
    }
}