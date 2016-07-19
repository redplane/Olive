using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseAppointmentFilter
    {
        /// <summary>
        ///     List of appointment which match with conditions.
        /// </summary>
        public IList<Appointment> Appointments { get; set; }

        /// <summary>
        ///     Total result which can be displayed to user.
        /// </summary>
        public int Total { get; set; }
    }
}