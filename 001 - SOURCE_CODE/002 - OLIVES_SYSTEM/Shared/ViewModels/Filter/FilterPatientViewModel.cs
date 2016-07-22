using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterPatientViewModel : FilterPersonViewModel, IPagination
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
        
        [NumericPropertyCompare("MaxWeight", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MinWeight { get; set; }

        [NumericPropertyCompare("MinWeight", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MaxWeight { get; set; }

        [NumericPropertyCompare("MaxHeight", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MinHeight { get; set; }

        [NumericPropertyCompare("MinHeight", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public float? MaxHeight { get; set; }

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}