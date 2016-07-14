using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterPlaceViewModel : IPagination
    {
        /// <summary>
        /// Name of city.
        /// </summary>
        [StringLength(FieldLength.CityNameMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string City { get; set; }

        /// <summary>
        /// Name of country
        /// </summary>
        [StringLength(FieldLength.CountryNameMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Country { get; set; }

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(new object[] { PlaceFilterSort.Id, PlaceFilterSort.City, PlaceFilterSort.Country }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public PlaceFilterSort Sort { get; set; } = PlaceFilterSort.Id;

        /// <summary>
        ///     Whether records are sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] {SortDirection.Decending, SortDirection.Ascending})]
        public SortDirection Direction { get; set; } = SortDirection.Ascending;

        /// <summary>
        ///     Index of result page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        /// <summary>
        ///     Number of record displayed on a page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Records { get; set; } = FieldLength.RecordMax;
    }
}