using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Models;

namespace Olives.Interfaces
{
    public interface IRepositoryRelationshipRequest
    {
        /// <summary>
        ///     Filter relationship requests by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseRelationshipRequestFilter> FilterRelationshipRequest(FilterRelationshipRequestViewModel filter);

        /// <summary>
        ///     Find the relationship request by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<RelationshipRequest> FindRelationshipRequest(FilterRelationshipRequestViewModel filter);

        /// <summary>
        ///     Initialize relationship request.
        /// </summary>
        /// <returns></returns>
        Task<RelationshipRequest> InitializeRelationshipRequest(RelationshipRequest relationshipRequest);

        /// <summary>
        ///     From relationship request to create a relationship between person and person.
        /// </summary>
        /// <param name="relationshipRequest"></param>
        /// <returns></returns>
        Task InitializeRelationship(RelationshipRequest relationshipRequest);

        /// <summary>
        ///     Delete relationship requests by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteRelationshipRequest(FilterRelationshipRequestViewModel filter);
    }
}