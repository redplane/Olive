using System;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNet.SignalR;
using Olives.Interfaces;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Services
{
    public class NotificationService : INotificationService
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of NotificationService.
        /// </summary>
        /// <param name="repositoryRealTimeConnection"></param>
        /// <param name="dataContext"></param>
        /// <param name="log"></param>
        public NotificationService(IRepositoryRealTimeConnection repositoryRealTimeConnection,
            IOliveDataContext dataContext, ILog log)
        {
            _repositoryRealTimeConnection = repositoryRealTimeConnection;
            _dataContext = dataContext;
            _log = log;
        }

        #endregion

        /// <summary>
        ///     This function is for broadcasting a notification to specified client.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="signalrHub">Hub which is used for broadcasting notification</param>
        /// <returns></returns>
        public async Task<Notification> BroadcastNotificationAsync(Notification notification, IHubContext signalrHub)
        {
            var context = _dataContext.Context;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Initialize the notification into database.
                    context.Notifications.Add(notification);

                    // Save the change asynchronously.
                    await context.SaveChangesAsync();

                    #region Notification broadcast

                    try
                    {
                        // Find all connections come from the recipient.
                        var connections =
                            await
                                _repositoryRealTimeConnection.FindRealTimeConnectionIndexesAsync(notification.Recipient,
                                    null, null);
                        signalrHub.Clients.Clients(connections).broadcastNotification(new
                        {
                            notification.Id,
                            notification.Type,
                            notification.Topic,
                            notification.Broadcaster,
                            notification.Recipient,
                            notification.Record,
                            notification.Message,
                            notification.Created
                        });
                    }
                    catch (Exception exception)
                    {
                        // As exception happens, log it and let the function continue running.
                        _log.Error(exception.Message, exception);
                    }

                    #endregion

                    // Submit the transaction.
                    transaction.Commit();

                    return notification;
                }
                catch (Exception exception)
                {
                    // Rollback the transaction.
                    transaction.Rollback();

                    // As exception happens, log it and let the function continue running.
                    _log.Error(exception.Message, exception);

                    return null;
                }
            }
        }

        #region Properties

        /// <summary>
        ///     Instance of logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Intstance which provides access to real time connection database.
        /// </summary>
        private readonly IRepositoryRealTimeConnection _repositoryRealTimeConnection;
        
        /// <summary>
        ///     Context which provides instance to access to database.
        /// </summary>
        private readonly IOliveDataContext _dataContext;

        #endregion
    }
}