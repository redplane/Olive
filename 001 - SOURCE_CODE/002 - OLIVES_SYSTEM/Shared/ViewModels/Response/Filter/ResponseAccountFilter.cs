using System.Collections.Generic;
using Shared.Models.Vertexes;

namespace Shared.ViewModels.Response.Filter
{
    public class ResponseAccountFilter
    {
        /// <summary>
        ///     List of filtered doctors.
        /// </summary>
        public IEnumerable<Account> Accounts { get; set; }

        /// <summary>
        ///     Total matched result.
        /// </summary>
        public int Total { get; set; }
    }
}