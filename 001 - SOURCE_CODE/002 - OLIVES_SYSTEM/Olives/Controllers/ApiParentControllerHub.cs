using System;
using Microsoft.AspNet.SignalR;

namespace Olives.Controllers
{
    public class ApiParentControllerHub<TTHub> : ApiParentController where TTHub : Hub
    {
        #region Properties

        /// <summary>
        /// Retrieve the hub context.
        /// </summary>
        readonly Lazy<IHubContext> _hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<TTHub>()
        );

        /// <summary>
        /// Return the hub context.
        /// </summary>
        public IHubContext Hub
        {
            get { return _hub.Value; }
        }

        #endregion
    }
}