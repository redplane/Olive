using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response.Personal
{
    public class ResponseBloodSugarFilter
    {
        /// <summary>
        ///     List of filtered heartbeat notes.
        /// </summary>
        public IEnumerable<BloodSugar> Sugarbloods { get; set; }

        /// <summary>
        ///     Number of results match the condition.
        /// </summary>
        public int Total { get; set; }
    }
}