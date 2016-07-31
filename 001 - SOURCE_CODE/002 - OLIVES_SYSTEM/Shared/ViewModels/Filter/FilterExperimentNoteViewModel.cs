using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterExperimentNoteViewModel : IPagination
    {
        /// <summary>
        ///     Medical record index
        /// </summary>
        public int? MedicalRecord { get; set; }

        /// <summary>
        ///     Name of medical experiment note.
        /// </summary>
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        /// <summary>
        ///     Person who sents the filter request.
        /// </summary>
        public int Requester { get; set; }

        /// <summary>
        ///     Partner who is included in medical experiment notes.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        ///     Mode of filter.
        /// </summary>
        public RecordFilterMode? Mode { get; set; }

        /// <summary>
        ///     Time after which experiment was lastly created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxCreate", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinCreated { get; set; }

        /// <summary>
        ///     Time before which experiment had been created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxCreated { get; set; }

        /// <summary>
        ///     Time after which experiment was lastly modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinLastModified { get; set; }

        /// <summary>
        ///     Time before which experiment had been modified.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxLastModified { get; set; }

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(
            new object[] {ExperimentFilterSort.Name, ExperimentFilterSort.Created, ExperimentFilterSort.LastModified},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public ExperimentFilterSort Sort { get; set; } = ExperimentFilterSort.LastModified;

        /// <summary>
        ///     Direction of sorting.
        /// </summary>
        [InEnumerationsArray(new object[] {SortDirection.Ascending, SortDirection.Decending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        /// <summary>
        ///     Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        /// <summary>
        ///     Maximum number of records a page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}