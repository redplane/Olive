using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class FilterDoctorViewModel : FilterPersonViewModel
    {
        public string Speciality { get; set; }

        [NumericPropertyCompare("MaxRank", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinRank { get; set; }

        [NumericPropertyCompare("MinRank", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxRank { get; set; }
    }
}