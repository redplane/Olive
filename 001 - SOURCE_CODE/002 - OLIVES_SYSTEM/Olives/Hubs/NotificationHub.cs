using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Olives.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Hubs
{
    [HubAuthorize]
    public class NotificationHub : Hub
    {
        /// <summary>
        /// Repository which provides function to access real time connection.
        /// </summary>
        public IRepositoryRealTimeConnection RepositoryRealTimeConnection => GlobalHost.DependencyResolver.Resolve<IRepositoryRealTimeConnection>();

        #region Overriden methods

        /// <summary>
        /// This function is called when a client connects to server by using hub connection.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            // Retrieve the connection index.
            var connectionIndex = Context.ConnectionId;

            // Take the email from request.
            var email = Context.Request.QueryString[Values.KeySignalrEmail];

            // Initialize a real time connection information.
            var realTimeConnection = new RealTimeConnection();
            realTimeConnection.Email = email;
            realTimeConnection.ConnectionId = connectionIndex;
            realTimeConnection.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

            // Initialize a connection information.
            RepositoryRealTimeConnection.InitializeRealTimeConnectionInfoAsync(realTimeConnection);

            return base.OnConnected();
        }

        /// <summary>
        /// This function is called when a client disconnects from server.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            // Email from signalr query string.
            var email = Context.Request.QueryString[Values.KeySignalrEmail];

            // Find and delete the connection by searching email and connection index.
            RepositoryRealTimeConnection.DeleteRealTimeConnectionInfoAsync(email,
                StringComparison.InvariantCultureIgnoreCase, Context.ConnectionId,
                StringComparison.InvariantCultureIgnoreCase);
            
            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region Methods

        /// <summary>
        /// This function is for notifying appointment changes to specific connections.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="type"></param>
        /// <param name="created"></param>
        /// <param name="appointment"></param>
        /// <returns></returns>
        public async Task NotifyAppointment(string email, AppointmentNotificationType type, double created, int appointment)
        {
            // Find all connections by using email.
            var realTimeConnections = await RepositoryRealTimeConnection.FindRealTimeConnectionInfoAsync(email, null);
            
            // Find all connection indexes.
            var connections = realTimeConnections.Select(x => x.ConnectionId).ToList();
            if (connections.Count < 1)
                return;

            // Notify the specific client about an appointment.
            await Clients.Clients(connections).notifyAppointment((byte) type, created, appointment);
        }

        #endregion
    }
}