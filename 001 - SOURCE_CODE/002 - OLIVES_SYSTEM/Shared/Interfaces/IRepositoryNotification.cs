using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryNotification
    {
        /// <summary>
        /// Initialize / update a notification to database.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task<Notification> InitializeNotificationAsync(Notification notification);

        /// <summary>
        /// Find notification by using index asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Notification> FindNotificationAsync(int id);

        /// <summary>
        /// Filter notification by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseNotificationFilter> FilterNotificationsAsync(FilterNotificationViewModel filter);
    }
}