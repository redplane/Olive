using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Models;
using Shared.ViewModels.Response;

namespace Olives.Interfaces
{
    public interface IRepositoryNotification
    {
        /// <summary>
        ///     Initialize / update a notification to database.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task<Notification> InitializeNotificationAsync(Notification notification);

        /// <summary>
        ///     Find notification by using index asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Notification> FindNotificationAsync(int id);

        /// <summary>
        ///     Filter notification by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseNotificationFilter> FilterNotificationsAsync(FilterNotificationViewModel filter);

        /// <summary>
        /// Make the notification matched with the conditions be seen.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> ConfirmNotificationSeen(FilterNotificationViewModel filter);
    }
}