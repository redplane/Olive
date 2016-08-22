using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response
{
    public class ResponseRelationshipFilter
    {
        /// <summary>
        ///     List of filtered relationships.
        /// </summary>
        public IEnumerable<Relationship> Relationships { get; set; }

        /// <summary>
        ///     Total of record matched with conditions.
        /// </summary>
        public int Total { get; set; }
    }
}