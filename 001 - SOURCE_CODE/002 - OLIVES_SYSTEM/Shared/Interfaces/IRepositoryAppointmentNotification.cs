using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryAppointmentNotification
    {
        /// <summary>
        ///     Initialize / update an appointment with parameters
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<AppointmentNotification> InitializeAppointmentNotificationAsync(AppointmentNotification initializer);

        /// <summary>
        ///     Filter appointment notifications with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseAppointmentNotificationFilter> FilterAppointmentNotificationAsync(
            FilterAppointmentNotificationViewModel filter);
    }
}