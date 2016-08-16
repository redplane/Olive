using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response
{
    public class ResponseAddictionFilter
    {
        /// <summary>
        ///     List of filtered allergies.
        /// </summary>
        public IEnumerable<Addiction> Addictions { get; set; }

        /// <summary>
        ///     Total records match the specific conditions.
        /// </summary>
        public int Total { get; set; }
    }
}