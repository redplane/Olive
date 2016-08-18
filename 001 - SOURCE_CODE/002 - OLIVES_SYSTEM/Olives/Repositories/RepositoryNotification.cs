using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Enumerations.Filter;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Repositories
{
    public class RepositoryNotification : IRepositoryNotification
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryNotification(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Filter notifications and make them to be seen.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> ConfirmNotificationSeen(FilterNotificationViewModel filter)
        {
            // By default, take all notification
            var context = _dataContext.Context;

            // Begin a transaction
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    IQueryable<Notification> notifications = context.Notifications;
                    notifications = FilterNotifications(notifications, filter);

                    // Make the filtered notifications be seen.
                    await notifications.ForEachAsync(x => { x.IsSeen = true; });

                    // Save the changes.
                    var records = await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return records;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

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
            notifications = FilterNotifications(notifications, filter);

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

        /// <summary>
        ///     Filter notifications by using specific conditions.
        /// </summary>
        /// <param name="notifications"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<Notification> FilterNotifications(IQueryable<Notification> notifications,
            FilterNotificationViewModel filter)
        {
            // Id is specified.
            if (filter.Id != null)
                notifications = notifications.Where(x => x.Id == filter.Id);

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
            if (filter.Types != null)
            {
                var types = new List<byte>(filter.Types);
                notifications = notifications.Where(x => types.Contains(x.Type));
            }
            // Notification topic filter.
            if (filter.Topics != null)
            {
                var topics = new List<byte>(filter.Topics);
                notifications = notifications.Where(x => topics.Contains(x.Topic));
            }
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

            return notifications;
        }

        #endregion
    }
}