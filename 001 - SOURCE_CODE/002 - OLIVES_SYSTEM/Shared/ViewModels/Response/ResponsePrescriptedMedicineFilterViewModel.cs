using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponsePrescriptedMedicineFilterViewModel
    {
        /// <summary>
        /// List of prescripted medicines.
        /// </summary>
        public IList<PrescriptedMedicine> PrescriptedMedicines { get; set; }

        /// <summary>
        /// Total matched results
        /// </summary>
        public int Total { get; set; }
    }
}