using System.Collections.Generic;

namespace Shared.ViewModels.Response
{
    public class ResponseHeartbeatFilter
    {
        /// <summary>
        /// List of filtered heartbeat notes.
        /// </summary>
        public IList<HeartbeatViewModel> Heartbeats { get; set; } 

        /// <summary>
        /// Number of results match the condition.
        /// </summary>
        public int Total { get; set; }
    }
}