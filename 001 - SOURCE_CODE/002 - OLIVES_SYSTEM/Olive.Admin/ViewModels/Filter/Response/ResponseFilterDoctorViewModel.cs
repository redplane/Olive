using System.Collections.Generic;

namespace Olive.Admin.ViewModels.Filter.Response
{
    public class ResponseFilterDoctorViewModel
    {
        /// <summary>
        /// Doctor information.
        /// </summary>
        public IEnumerable<DoctorViewModel> Doctors { get; set; }

        /// <summary>
        /// Total filtered result.
        /// </summary>
        public int Total { get; set; }
    }
}