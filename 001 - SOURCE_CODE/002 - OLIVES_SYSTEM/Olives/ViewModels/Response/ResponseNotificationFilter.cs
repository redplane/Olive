using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response
{
    public class ResponseNotificationFilter
    {
        /// <summary>
        ///     List of filtered appointments.
        /// </summary>
        public IEnumerable<Notification> Notifications { get; set; }

        /// <summary>
        ///     Total condition matched records.
        /// </summary>
        public int Total { get; set; }
    }
}