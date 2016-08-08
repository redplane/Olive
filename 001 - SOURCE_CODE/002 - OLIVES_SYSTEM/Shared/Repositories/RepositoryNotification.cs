using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryNotification : IRepositoryNotification
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Properties

        public RepositoryNotification(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Filter notifications by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseNotificationFilter> FilterNotificationsAsync(FilterNotificationViewModel filter)
        {
            // By default, take all notification
            var context = _dataContext.Context;
            IQueryable<Notification> notifications = context.Notifications;

            // Filter by requester mode.

            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                notifications = notifications.Where(x => x.Recipient == filter.Requester);
                if (filter.Partner != null)
                    notifications = notifications.Where(x => x.Broadcaster == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                notifications = notifications.Where(x => x.Broadcaster == filter.Requester);
                if (filter.Partner != null)
                    notifications = notifications.Where(x => x.Recipient == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    notifications =
                        notifications.Where(x => x.Broadcaster == filter.Requester || x.Recipient == filter.Requester);
                else
                    notifications =
                        notifications.Where(
                            x =>
                                (x.Broadcaster == filter.Requester && x.Recipient == filter.Partner.Value) ||
                                (x.Broadcaster == filter.Partner.Value && x.Recipient == filter.Requester));
            }

            // Notification type filter.
            if (filter.Type != null)
                notifications = notifications.Where(x => x.Type == (byte) filter.Type.Value);

            // Notification topic filter.
            if (filter.Topic != null)
                notifications = notifications.Where(x => x.Topic == (byte) filter.Topic.Value);

            // Record is specified.
            if (filter.Record != null)
                notifications = notifications.Where(x => x.Record == filter.Record.Value);

            // Created is defined.
            if (filter.MinCreated != null)
                notifications = notifications.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null)
                notifications = notifications.Where(x => x.Created <= filter.MaxCreated.Value);

            if (filter.IsSeen != null)
                notifications = notifications.Where(x => x.IsSeen == filter.IsSeen.Value);

            // Result sort.
            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case NotificationFilterSort.Created:
                            notifications = notifications.OrderByDescending(x => x.Created);
                            break;
                        case NotificationFilterSort.IsSeen:
                            notifications = notifications.OrderByDescending(x => x.IsSeen);
                            break;
                        case NotificationFilterSort.Topic:
                            notifications = notifications.OrderByDescending(x => x.Topic);
                            break;
                        case NotificationFilterSort.Type:
                            notifications = notifications.OrderByDescending(x => x.Type);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case NotificationFilterSort.Created:
                            notifications = notifications.OrderBy(x => x.Created);
                            break;
                        case NotificationFilterSort.IsSeen:
                            notifications = notifications.OrderBy(x => x.IsSeen);
                            break;
                        case NotificationFilterSort.Topic:
                            notifications = notifications.OrderBy(x => x.Topic);
                            break;
                        case NotificationFilterSort.Type:
                            notifications = notifications.OrderBy(x => x.Type);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseNotificationFilter();
            response.Total = await notifications.CountAsync();

            // Pagination.
            if (filter.Records != null)
                notifications = notifications.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);

            response.Notifications = notifications;

            return response;
        }

        /// <summary>
        ///     Find a notification by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Notification> FindNotificationAsync(int id)
        {
            // Find the first notification with the matched id.
            var context = _dataContext.Context;
            return await context.Notifications.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Initialize / update notification asynchronously.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task<Notification> InitializeNotificationAsync(Notification notification)
        {
            var context = _dataContext.Context;

            // Save or update data.
            context.Notifications.AddOrUpdate(notification);

            // Save changes.
            await context.SaveChangesAsync();

            return notification;
        }

        #endregion
    }
}