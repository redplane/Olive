using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseAllergyFilter
    {
        /// <summary>
        ///     List of filtered allergies.
        /// </summary>
        public IList<Allergy> Allergies { get; set; }

        /// <summary>
        ///     Total records match the specific conditions.
        /// </summary>
        public int Total { get; set; }
    }
}