using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Enumerations.Filter;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Repositories
{
    public class RepositoryRelationshipRequest : IRepositoryRelationshipRequest
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance with context wrapper.
        /// </summary>
        /// <param name="oliveDataContext"></param>
        /// <param name="timeService"></param>
        public RepositoryRelationshipRequest(IOliveDataContext oliveDataContext, ITimeService timeService)
        {
            _oliveDataContext = oliveDataContext;
            _timeService = timeService;
        }

        #endregion

        /// <summary>
        ///     Find relationship with specific conditions and delete 'em.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteRelationshipRequest(FilterRelationshipRequestViewModel filter)
        {
            // Initialize database context.
            var context = _oliveDataContext.Context;

            // Take all records.
            IQueryable<RelationshipRequest> relationshipRequests = context.RelationshipRequests;

            // Filter the relationship requests.
            relationshipRequests = FilterRelationshipRequests(relationshipRequests, filter);

            // Delete all filtered relationship.
            context.RelationshipRequests.RemoveRange(relationshipRequests);

            // Save changes.
            return await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Filter reltionship requests by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseRelationshipRequestFilter> FilterRelationshipRequest(
            FilterRelationshipRequestViewModel filter)
        {
            // By default, take all records.
            var context = _oliveDataContext.Context;
            IQueryable<RelationshipRequest> relationshipRequests = context.RelationshipRequests;
            relationshipRequests = FilterRelationshipRequests(relationshipRequests, filter);

            // Result sorting
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case RelationshipRequestFilterSort.Created:
                            relationshipRequests = relationshipRequests.OrderBy(x => x.Created);
                            break;
                        default:
                            relationshipRequests = relationshipRequests.OrderBy(x => x.LastModified);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case RelationshipRequestFilterSort.Created:
                            relationshipRequests = relationshipRequests.OrderByDescending(x => x.Created);
                            break;
                        default:
                            relationshipRequests = relationshipRequests.OrderByDescending(x => x.LastModified);
                            break;
                    }
                    break;
            }

            // Calculate the total result.
            var response = new ResponseRelationshipRequestFilter();
            response.Total = await relationshipRequests.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                relationshipRequests = relationshipRequests.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            response.RelationshipRequests = await relationshipRequests.ToListAsync();

            return response;
        }

        /// <summary>
        ///     Find the relationship request by using specific condition.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<RelationshipRequest> FindRelationshipRequest(FilterRelationshipRequestViewModel filter)
        {
            // Initialize context.
            var context = _oliveDataContext.Context;

            // By default, take the whole list.
            IQueryable<RelationshipRequest> relationshipRequests = context.RelationshipRequests;

            // Filter records and take the first one.
            relationshipRequests = FilterRelationshipRequests(relationshipRequests, filter);

            return await relationshipRequests.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     From the relationship request to initialize relationship.
        /// </summary>
        /// <param name="relationshipRequest"></param>
        /// <returns></returns>
        public async Task InitializeRelationship(RelationshipRequest relationshipRequest)
        {
            // Context initialization.
            var context = _oliveDataContext.Context;

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Initialize the relationship.
                    var relation = new Relation();
                    relation.Source = relationshipRequest.Source;
                    relation.Target = relationshipRequest.Target;
                    relation.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                    // Add/update relationship.
                    context.Relations.AddOrUpdate(x => new
                    {
                        x.Source,
                        x.Target
                    }, relation);

                    // Remove the relationship request.
                    context.RelationshipRequests.RemoveRange(context.RelationshipRequests.Where(x => x.Id == relationshipRequest.Id));

                    // Save changes.
                    await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        ///     Initialize relationship request.
        /// </summary>
        /// <param name="relationshipRequest"></param>
        /// <returns></returns>
        public async Task<RelationshipRequest> InitializeRelationshipRequest(RelationshipRequest relationshipRequest)
        {
            // Initialize database context.
            var context = _oliveDataContext.Context;

            context.RelationshipRequests.AddOrUpdate(relationshipRequest);
            await context.SaveChangesAsync();

            return relationshipRequest;
        }

        /// <summary>
        ///     Filter medical records by using specific conditions.
        /// </summary>
        /// <param name="relationshipRequests"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<RelationshipRequest> FilterRelationshipRequests(
            IQueryable<RelationshipRequest> relationshipRequests, FilterRelationshipRequestViewModel filter)
        {
            // Base on requester role, do the filter.
            relationshipRequests = FilterRelationshipRequestsByRequesterRole(relationshipRequests, filter);

            // Id is specified.
            if (filter.Id != null)
                relationshipRequests = relationshipRequests.Where(x => x.Id == filter.Id);

            // Content is specified.
            if (!string.IsNullOrWhiteSpace(filter.Content))
                relationshipRequests = relationshipRequests.Where(x => x.Content.Contains(filter.Content));

            // Created is specified.
            if (filter.MinCreated != null)
                relationshipRequests = relationshipRequests.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                relationshipRequests = relationshipRequests.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is specified.
            if (filter.MinLastModified != null)
                relationshipRequests = relationshipRequests.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                relationshipRequests = relationshipRequests.Where(x => x.LastModified <= filter.MaxLastModified);

            return relationshipRequests;
        }

        /// <summary>
        ///     Base on the requester role to do exact filter function.
        /// </summary>
        /// <param name="relationshipRequests"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<RelationshipRequest> FilterRelationshipRequestsByRequesterRole(
            IQueryable<RelationshipRequest> relationshipRequests,
            FilterRelationshipRequestViewModel filter)
        {
            // Requester is not defined.
            if (filter.Requester == null)
                throw new Exception("Requester must be specified.");

            // Patient is always the relationship request sender.
            if (filter.Requester.Role == (byte) Role.Patient)
            {
                // Find the source by using patient id.
                relationshipRequests = relationshipRequests.Where(x => x.Source == filter.Requester.Id);

                // Partner is specified.
                if (filter.Partner != null)
                    relationshipRequests = relationshipRequests.Where(x => x.Target == filter.Partner.Value);

                return relationshipRequests;
            }

            // Doctor is always the relationship request receiver.
            relationshipRequests = relationshipRequests.Where(x => x.Target == filter.Requester.Id);

            // Partner is specified. Find the source of relationship request.
            if (filter.Partner != null)
                relationshipRequests = relationshipRequests.Where(x => x.Source == filter.Partner.Value);

            return relationshipRequests;
        }

        #region Properties

        /// <summary>
        ///     Data context wrapper which contains context to access database.
        /// </summary>
        private readonly IOliveDataContext _oliveDataContext;

        /// <summary>
        ///     Service which provides functions to access time calculation functions.
        /// </summary>
        private readonly ITimeService _timeService;

        #endregion
    }
}