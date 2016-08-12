using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterMedicalRecordViewModel : IPagination
    {
        /// <summary>
        /// Id of medical record.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Who sent the filter request.
        /// </summary>
        [BindNever]
        public Person Requester { get; set; }

        /// <summary>
        ///     Who is included in medical record.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        ///     Filtering mode.
        /// </summary>
        public RecordFilterMode? Mode { get; set; }
        
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxTime", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public float? MinTime { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinTime", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public float? MaxTime { get; set; }

        /// <summary>
        ///     Category of medical record.
        /// </summary>
        public int? Category { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinCreated { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxCreated { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public float? MinLastModified { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public float? MaxLastModified { get; set; }

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        public MedicalRecordFilterSort Sort { get; set; } = MedicalRecordFilterSort.LastModified;

        /// <summary>
        ///     Whether record should be sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}