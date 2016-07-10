using System.Collections.Generic;

namespace Shared.ViewModels.Response
{
    public class ResponseRelativeFilter
    {
        /// <summary>
        /// Related doctors
        /// </summary>
        public IList<RelativeViewModel> List { get; set; }

        /// <summary>
        /// Total matched records.
        /// </summary>
        public int Total { get; set; }
    }
}