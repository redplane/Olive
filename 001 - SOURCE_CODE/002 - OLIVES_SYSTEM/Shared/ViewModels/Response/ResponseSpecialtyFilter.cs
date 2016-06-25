using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseSpecialtyFilter
    {
        /// <summary>
        /// List of specialties.
        /// </summary>
        public IList<Specialty> Specialties { get; set; }

        /// <summary>
        /// How many specialties match with conditions.
        /// </summary>
        public int Total { get; set; }
    }
}