using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterDoctorViewModel : FilterPersonViewModel, IPagination
    {
        /// <summary>
        ///     Amount of money user's must be higher than.
        /// </summary>
        [NumericPropertyCompare("MaxMoney", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinMoney { get; set; }

        /// <summary>
        ///     Amount of money user's must be lower than.
        /// </summary>
        [NumericPropertyCompare("MinMoney", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxMoney { get; set; }


        /// <summary>
        ///     Id of city where doctor lives.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int? City { get; set; }

        /// <summary>
        ///     Id of doctor's specialty.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int? Specialty { get; set; }

        [Range(0, 5, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MaxRank", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinRank { get; set; }

        [Range(0, 5, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        [NumericPropertyCompare("MinRank", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxRank { get; set; }

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = FieldLength.PageIndexMin;

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Records { get; set; } = FieldLength.RecordMax;
    }
}