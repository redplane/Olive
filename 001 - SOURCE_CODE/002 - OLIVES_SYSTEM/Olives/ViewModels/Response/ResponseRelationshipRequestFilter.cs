using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response
{
    public class ResponseRelationshipRequestFilter
    {
        /// <summary>
        /// List of filtered relationship requests.
        /// </summary>
        public IEnumerable<RelationshipRequest> RelationshipRequests { get; set; }  

        /// <summary>
        /// Total result
        /// </summary>
        public int Total { get; set; }
    }
}