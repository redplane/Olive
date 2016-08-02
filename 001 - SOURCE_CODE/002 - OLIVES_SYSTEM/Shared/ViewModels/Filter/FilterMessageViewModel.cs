using System;
using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterMessageViewModel : IPagination
    {
        /// <summary>
        /// Person who sent the filter request.
        /// </summary>
        public int Requester { get; set; } 

        /// <summary>
        /// Who takes part in the message.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        /// Whether the partner is the message broadcaster or message receiver.
        /// </summary>
        public RecordFilterMode Mode { get; set; }

        /// <summary>
        /// Time after which message was created.
        /// </summary>
        public double? MinCreated { get; set; }

        /// <summary>
        /// Time before which message had been created.
        /// </summary>
        public double? MaxCreated { get; set; }

        /// <summary>
        /// Whether message is seen or not.
        /// </summary>
        public bool? IsSeen { get; set; }

        /// <summary>
        /// Whether records should be sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        /// <summary>
        /// Which property should be used for sorting.
        /// </summary>
        public MessageFilterSort Sort { get; set; } = MessageFilterSort.Created;

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}