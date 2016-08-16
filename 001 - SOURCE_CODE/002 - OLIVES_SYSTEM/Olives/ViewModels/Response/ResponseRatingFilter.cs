using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response
{
    public class ResponseRatingFilter
    {
        /// <summary>
        ///     List of filtered rates.
        /// </summary>
        public IList<Rating> Rates { get; set; }

        /// <summary>
        ///     Total records match with the filter conditions.
        /// </summary>
        public int Total { get; set; }
    }
}