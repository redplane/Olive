using System.ComponentModel.DataAnnotations;
using Olives.Enumerations.Filter;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Olives.ViewModels.Filter
{
    public class FilterRatingViewModel : IPagination
    {
        /// <summary>
        ///     Requester who sent the filter request.
        /// </summary>
        public int Requester { get; set; }

        /// <summary>
        ///     Who is included in rating.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        ///     Filtering mode
        /// </summary>
        [InEnumerationsArray(new object[] {RecordFilterMode.RequesterIsCreator, RecordFilterMode.RequesterIsOwner},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public RecordFilterMode? Mode { get; set; }

        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MinCreated { get; set; }

        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MaxCreated { get; set; }

        /// <summary>
        ///     Value which rating must be greater than.
        /// </summary>
        [Range(1, 5, ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MaxProperty", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public byte? MinValue { get; set; }

        /// <summary>
        ///     Value which rating must be smaller than.
        /// </summary>
        [Range(1, 5, ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MinProperty", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public byte? MaxValue { get; set; }

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(
            new object[] {RatingResultSort.Value, RatingResultSort.Created},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public RatingResultSort Sort { get; set; } = RatingResultSort.Value;

        /// <summary>
        ///     Whether records should be sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        /// <summary>
        ///     Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        /// <summary>
        ///     Number of record displayed on a page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}