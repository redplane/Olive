using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using System.Web.ModelBinding;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Olives.ViewModels.Filter
{
    public class FilterRelationshipViewModel : IPagination
    {
        /// <summary>
        /// Id of request.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Who sent the filter request.
        /// </summary>
        [HttpBindNever]
        [BindNever]
        public Person Requester { get; set; }

        /// <summary>
        ///     Who is included in medical record.
        /// </summary>
        public int? Partner { get; set; }
        
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
        
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}