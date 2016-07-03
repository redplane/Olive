using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterCityViewModel : IPagination
    {
        public int? Id { get; set; }
        
        public string Name { get; set; }

        public int? CountryId { get; set; }

        public string CountryName { get; set; }
        
        /// <summary>
        /// Which property should be used for sorting.
        /// </summary>
        public CityResultSort Sort { get; set; }

        /// <summary>
        /// Whether records are sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection Direction { get; set; }

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
           ErrorMessageResourceType = typeof(Language),
           ErrorMessageResourceName = "InvalidPageIndex")]
        public int Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Records { get; set; } = FieldLength.RecordMax;
    }
}