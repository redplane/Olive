using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class FilterPatientViewModel : FilterPersonViewModel
    {
        [NumericPropertyCompare("MaxWeight", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMinWeight")]
        public float? MinWeight { get; set; }

        [NumericPropertyCompare("MinWeight", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMaxWeight")]
        public float? MaxWeight { get; set; }

        [NumericPropertyCompare("MaxHeight", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMinHeight")]
        public float? MinHeight { get; set; }

        [NumericPropertyCompare("MinHeight", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMaxHeight")]
        public float? MaxHeight { get; set; }
    }
}