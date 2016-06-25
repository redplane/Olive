using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseBloodPressureFilter
    {
        /// <summary>
        /// List of specialties.
        /// </summary>
        public IList<BloodPressureViewModel> BloodPressures { get; set; }

        /// <summary>
        /// How many specialties match with conditions.
        /// </summary>
        public int Total { get; set; }
    }
}