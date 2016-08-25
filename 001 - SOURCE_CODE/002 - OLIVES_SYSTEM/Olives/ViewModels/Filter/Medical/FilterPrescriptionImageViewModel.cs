using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Olives.ViewModels.Filter.Medical
{
    public class FilterPrescriptionImageViewModel : IPagination
    {
        /// <summary>
        ///     Id of prescription image.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Who send the filter request.
        /// </summary>
        public Person Requester { get; set; }

        /// <summary>
        ///     Who created prescription image or just own it.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        ///     Index of prescription.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int? Prescription { get; set; }

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

        /// <summary>
        ///     Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = FieldLength.PageIndexMin;

        /// <summary>
        ///     Amount of records per page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}