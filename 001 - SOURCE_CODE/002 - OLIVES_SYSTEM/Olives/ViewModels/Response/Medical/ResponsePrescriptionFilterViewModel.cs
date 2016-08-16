using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response.Medical
{
    public class ResponsePrescriptionFilterViewModel
    {
        /// <summary>
        ///     List of filtered prescription.
        /// </summary>
        public IList<Prescription> Prescriptions { get; set; }

        /// <summary>
        ///     Total matched results.
        /// </summary>
        public int Total { get; set; }
    }
}