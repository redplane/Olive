using System.Collections.Generic;
using Olives.ViewModels.Filter;

namespace Olives.ViewModels.Response
{
    public class ResponseRelatedDoctorFilter
    {
        /// <summary>
        ///     Related doctors
        /// </summary>
        public IList<RelatedDoctorViewModel> List { get; set; }

        /// <summary>
        ///     Total matched records.
        /// </summary>
        public int Total { get; set; }
    }
}