using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterPrescriptionImageViewModel : IPagination
    {
        /// <summary>
        /// Who send the filter request.
        /// </summary>
        public int Requester { get; set; }

        /// <summary>
        /// Who created prescription image or just own it.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        /// Mode of prescription image filter.
        /// </summary>
        public RecordFilterMode? Mode { get; set; }
        
        /// <summary>
        /// Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = FieldLength.PageIndexMin;

        /// <summary>
        /// Amount of records per page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}