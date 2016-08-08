using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseDoctorFilter
    {
        /// <summary>
        ///     List of filtered doctors.
        /// </summary>
        public IEnumerable<Doctor> Doctors { get; set; }

        /// <summary>
        ///     Total matched result.
        /// </summary>
        public int Total { get; set; }
    }
}