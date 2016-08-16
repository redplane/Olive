using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response.Personal
{
    public class ResponseAllergyFilter
    {
        /// <summary>
        ///     List of filtered allergies.
        /// </summary>
        public IEnumerable<Allergy> Allergies { get; set; }

        /// <summary>
        ///     Total records match the specific conditions.
        /// </summary>
        public int Total { get; set; }
    }
}