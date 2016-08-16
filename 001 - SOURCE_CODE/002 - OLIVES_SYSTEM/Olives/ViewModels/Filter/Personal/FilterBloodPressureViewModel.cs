using System.ComponentModel.DataAnnotations;
using Olives.Enumerations.Filter;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Olives.ViewModels.Filter.Personal
{
    public class FilterBloodPressureViewModel : IPagination
    {
        /// <summary>
        ///     Id of note.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Owner id of heartbeat.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        ///     Value which diastolic must be greater than.
        /// </summary>
        [Range(Values.MinDiastolic, Values.MaxDiastolic, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MaxDiastolic", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public int? MinDiastolic { get; set; }

        /// <summary>
        ///     Value which diastolic must be smaller than.
        /// </summary>
        [Range(Values.MinDiastolic, Values.MaxDiastolic, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MinDiastolic", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public int? MaxDiastolic { get; set; }

        /// <summary>
        ///     Value which systolic must be greater than.
        /// </summary>
        [Range(Values.MinSystolic, Values.MaxSystolic, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MaxSystolic", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public int? MinSystolic { get; set; }

        /// <summary>
        ///     Value which systolic must be smaller than.
        /// </summary>
        [Range(Values.MinSystolic, Values.MaxSystolic, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MinSystolic", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public int? MaxSystolic { get; set; }

        /// <summary>
        ///     Note of blood pressure measurement record.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }

        [NumericPropertyCompare("MaxTime", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MinTime { get; set; }

        [NumericPropertyCompare("MinTime", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MaxTime { get; set; }

        /// <summary>
        ///     Time after which note was created.
        /// </summary>
        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MinCreated { get; set; }

        /// <summary>
        ///     Time before which note had been created.
        /// </summary>
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MaxCreated { get; set; }

        /// <summary>
        ///     Time after which note was lastly modified.
        /// </summary>
        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MinLastModified { get; set; }

        /// <summary>
        ///     Time after which note had been lastly modified.
        /// </summary>
        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MaxLastModified { get; set; }

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        public NoteResultSort Sort { get; set; } = NoteResultSort.Time;

        /// <summary>
        ///     Whether record should be sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        /// <summary>
        ///     Index of page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; }

        /// <summary>
        ///     Number of records which can be shown up.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}