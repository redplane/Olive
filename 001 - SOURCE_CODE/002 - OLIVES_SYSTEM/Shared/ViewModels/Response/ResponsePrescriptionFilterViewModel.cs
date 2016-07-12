using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
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