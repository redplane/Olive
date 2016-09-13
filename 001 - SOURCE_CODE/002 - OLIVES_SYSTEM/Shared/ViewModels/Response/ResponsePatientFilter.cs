using System.Collections.Generic;
using Shared.Models.Vertexes;

namespace Shared.ViewModels.Response
{
    public class ResponsePatientFilter
    {
        /// <summary>
        ///     List of filtered patients.
        /// </summary>
        public IEnumerable<Patient> Patients { get; set; }

        /// <summary>
        ///     Total filtered result number.
        /// </summary>
        public int Total { get; set; }
    }
}