using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseRelationshipFilter
    {
        /// <summary>
        ///     List of filtered relationships.
        /// </summary>
        public IEnumerable<Relation> Relationships { get; set; }

        /// <summary>
        ///     Total of record matched with conditions.
        /// </summary>
        public int Total { get; set; }
    }
}