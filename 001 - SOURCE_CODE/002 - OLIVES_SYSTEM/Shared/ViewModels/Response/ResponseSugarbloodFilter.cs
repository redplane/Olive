using System.Collections.Generic;

namespace Shared.ViewModels.Response
{
    public class ResponseSugarbloodFilter
    {
        /// <summary>
        /// List of filtered heartbeat notes.
        /// </summary>
        public IList<SugarbloodViewModel> Sugarbloods { get; set; } 

        /// <summary>
        /// Number of results match the condition.
        /// </summary>
        public int Total { get; set; }
    }
}