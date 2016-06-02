using System.Collections.Generic;

namespace Shared.ViewModels
{
    public class ResponseViewModel
    {
        /// <summary>
        ///     Error messages (not null as available).
        /// </summary>
        public IEnumerable<string> Errors { get; set; }

        /// <summary>
        ///     Response data (not null as available)
        /// </summary>
        public object Data { get; set; }
    }
}