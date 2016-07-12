using System.Collections.Generic;

namespace Shared.ViewModels.Response
{
    public class ResponseAppointmentFilter
    {
        /// <summary>
        ///     List of appointment which match with conditions.
        /// </summary>
        public IList<AppointmentViewModel> Appointments { get; set; }

        /// <summary>
        ///     Total result which can be displayed to user.
        /// </summary>
        public int Total { get; set; }
    }
}