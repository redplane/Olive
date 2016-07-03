using System.Collections.Generic;

namespace Shared.ViewModels.Response
{
    public class ResponseCityFilter
    {
        /// <summary>
        /// List of filtered allergies.
        /// </summary>
        public IList<CityViewModel> Cities { get; set; }

        /// <summary>
        /// Total records match the specific conditions.
        /// </summary>
        public int Total { get; set; }
    }
}