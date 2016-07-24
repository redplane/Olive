using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterMedicalImageViewModel : IPagination
    {
        /// <summary>
        /// Id of person who sent the filter request.
        /// </summary>
        public int Requester { get; set; }

        /// <summary>
        /// Person who created / received the image.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        /// Id of medical record which image belongs to.
        /// </summary>
        public int MedicalRecord { get; set; }
        
        /// <summary>
        ///     Whether records are sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection Direction { get; set; } = SortDirection.Ascending;

        /// <summary>
        /// Mode of filtering.
        /// </summary>
        public RecordFilterMode? Mode { get; set; }

        /// <summary>
        /// Index of page result.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        /// <summary>
        ///     Maximum number of records a page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}