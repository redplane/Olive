using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseAppointmentNotificationFilter
    {
        /// <summary>
        /// List of filtered appointments.
        /// </summary>
        public IList<AppointmentNotification> AppointmentNotifications { get; set; } 

        /// <summary>
        /// Total condition matched records.
        /// </summary>
        public int Total { get; set; }
    }
}