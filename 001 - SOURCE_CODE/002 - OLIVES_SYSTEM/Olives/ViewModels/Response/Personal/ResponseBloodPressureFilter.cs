using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response.Personal
{
    public class ResponseBloodPressureFilter
    {
        /// <summary>
        ///     List of specialties.
        /// </summary>
        public IEnumerable<BloodPressure> BloodPressures { get; set; }

        /// <summary>
        ///     How many specialties match with conditions.
        /// </summary>
        public int Total { get; set; }
    }
}