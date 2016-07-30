using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseBloodSugarFilter
    {
        /// <summary>
        ///     List of filtered heartbeat notes.
        /// </summary>
        public IEnumerable<SugarBlood> Sugarbloods { get; set; }

        /// <summary>
        ///     Number of results match the condition.
        /// </summary>
        public int Total { get; set; }
    }
}