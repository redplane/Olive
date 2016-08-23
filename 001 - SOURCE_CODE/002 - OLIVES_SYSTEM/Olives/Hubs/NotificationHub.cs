using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Olives.Attributes;
using Olives.Interfaces;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Hubs
{
    [HubAuthorize]
    public class NotificationHub : Hub
    {
        /// <summary>
        ///     Repository which provides function to access real time connection.
        /// </summary>
        private IRepositoryRealTimeConnection RepositoryRealTimeConnection
            => GlobalHost.DependencyResolver.Resolve<IRepositoryRealTimeConnection>();

        /// <summary>
        ///     Service which provides functions to access time calculation.
        /// </summary>
        private ITimeService TimeService => GlobalHost.DependencyResolver.Resolve<ITimeService>();

        #region Overriden methods

        /// <summary>
        ///     This function is called when a client connects to server by using hub connection.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            // Retrieve the account information.
            var account = (Person) Context.Request.Environment[Values.KeySignalrClient];

            // Retrieve the connection index.
            var connectionIndex = Context.ConnectionId;

            // Initialize a real time connection information.
            var realTimeConnection = new RealTimeConnection();
            realTimeConnection.Owner = account.Id;
            realTimeConnection.ConnectionId = connectionIndex;
            realTimeConnection.Created = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Initialize a connection information.
            RepositoryRealTimeConnection.InitializeRealTimeConnectionInfoAsync(realTimeConnection);

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            // Retrieve the account information.
            var account = (Person)Context.Request.Environment[Values.KeySignalrClient];

            // Retrieve the connection index.
            var connectionIndex = Context.ConnectionId;

            // Initialize a real time connection information.
            var realTimeConnection = new RealTimeConnection();
            realTimeConnection.Owner = account.Id;
            realTimeConnection.ConnectionId = connectionIndex;
            realTimeConnection.Created = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Initialize a connection information.
            RepositoryRealTimeConnection.InitializeRealTimeConnectionInfoAsync(realTimeConnection);
            
            return base.OnReconnected();
        }
        /// <summary>
        ///     This function is called when a client disconnects from server.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            // Retrieve the account information.
            var account = (Person) Context.Request.Environment[Values.KeySignalrClient];

            // Find and delete the connection by searching email and connection index.
            RepositoryRealTimeConnection.DeleteRealTimeConnectionInfoAsync(account.Id, Context.ConnectionId,
                StringComparison.InvariantCultureIgnoreCase);

            return base.OnDisconnected(stopCalled);
        }

        #endregion
    }
}