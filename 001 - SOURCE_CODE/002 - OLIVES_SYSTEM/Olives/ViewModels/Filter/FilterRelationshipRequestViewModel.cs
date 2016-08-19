using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using System.Web.ModelBinding;
using Olives.Constants;
using Olives.Enumerations.Filter;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Models;
using Shared.Resources;

namespace Olives.ViewModels.Filter
{
    public class FilterRelationshipRequestViewModel
    {
        /// <summary>
        ///     Id of relationship request.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Who sent the filter request.
        /// </summary>
        [BindNever]
        [HttpBindNever]
        public Person Requester { get; set; }

        /// <summary>
        ///     Who is included in medical record.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        ///     Content of request.
        /// </summary>
        [StringLength(OlivesValues.MaxRelationshipRequestContentLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Content { get; set; }

        /// <summary>
        ///     Time after which relationship which was created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinCreated { get; set; }

        /// <summary>
        ///     Time before which relationship which had been created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxCreated { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeGreaterThan")]
        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinLastModified { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeGreaterThan")]
        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MaxLastModified { get; set; }

        /// <summary>
        ///     Whether records should be sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] {SortDirection.Ascending, SortDirection.Decending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public SortDirection Direction { get; set; }

        /// <summary>
        ///     Which property should be used for sorting,
        /// </summary>
        [InEnumerationsArray(
            new object[] {RelationshipRequestFilterSort.Created, RelationshipRequestFilterSort.LastModified},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public RelationshipRequestFilterSort Sort { get; set; }

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}