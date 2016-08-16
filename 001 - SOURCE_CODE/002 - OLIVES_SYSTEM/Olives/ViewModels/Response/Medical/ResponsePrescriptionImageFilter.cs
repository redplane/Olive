using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response.Medical
{
    public class ResponsePrescriptionImageFilter
    {
        /// <summary>
        ///     List of filtered prescription images.
        /// </summary>
        public IList<PrescriptionImage> PrescriptionImages { get; set; }

        /// <summary>
        ///     Total records which match with conditions.
        /// </summary>
        public int Total { get; set; }
    }
}