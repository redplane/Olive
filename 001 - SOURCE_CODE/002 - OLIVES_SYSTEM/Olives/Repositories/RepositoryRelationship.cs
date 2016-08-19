using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces;
using Olives.ViewModels;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Repositories
{
    public class RepositoryRelationship : IRepositoryRelationship
    {
        #region Properties

        /// <summary>
        ///     Context wrapper which contains context to access to database.
        /// </summary>
        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of repository which contains context wrapper.
        /// </summary>
        /// <param name="dataContext"></param>
        public RepositoryRelationship(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find the relationship with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// >
        /// <returns></returns>
        public async Task<Relation> FindRelationshipAsync(FilterRelationshipViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            // Take all relationships.
            IQueryable<Relation> relationships = context.Relations;

            // Filter the relationships.
            relationships = FilterRelationships(relationships, filter);

            // Return the first queried result.
            return await relationships.FirstOrDefaultAsync();
        }
        
        /// <summary>
        ///     Delete a relation asynchronously.
        /// </summary>
        /// <param name="filter">Id of relationship</param>
        /// <returns></returns>
        public async Task<int> DeleteRelationshipAsync(FilterRelationshipViewModel filter)
        {
            var context = _dataContext.Context;

            // By default, take all relationships.
            IQueryable<Relation> relationships = context.Relations;

            // Filter the relationships.
            relationships = FilterRelationships(relationships, filter);


            // Find the relation whose id is matched and has the specific person takes part in.
            context.Relations.RemoveRange(relationships);

            return await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Filter relationship base on the role of requester.
        /// </summary>
        /// <param name="filter"></param>
        public async Task<ResponseRelationshipFilter> FilterRelationshipAsync(FilterRelationshipViewModel filter)
        {
            var context = _dataContext.Context;

            // By default, take all relationship.
            IQueryable<Relation> relationships = context.Relations;
            relationships = FilterRelationships(relationships, filter);

            // Response initialization.Filter
            var response = new ResponseRelationshipFilter();
            response.Total = await relationships.CountAsync();

            if (filter.Records != null)
                relationships = relationships.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);

            response.Relationships = await relationships.ToListAsync();
            return response;
        }

        /// <summary>
        ///     Filter related doctors.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<ResponseRelatedDoctorFilter> FilterRelatedDoctorAsync(int requester,
            int page, int? records)
        {
            var context = _dataContext.Context;

            // By default, take all relationship.
            IQueryable<Relation> relationships = context.Relations;

            // Take the relationship whose source is requester and type is provide treatment.
            relationships = relationships.Where(x => x.Source == requester);

            // Take all people who are doctor.
            IQueryable<Doctor> doctors = context.Doctors;

            var fullResult = from r in relationships
                join d in doctors on r.Target equals d.Id
                select new RelatedDoctorViewModel
                {
                    Relation = r.Id,
                    Doctor = d,
                    Created = r.Created
                };

            var response = new ResponseRelatedDoctorFilter();
            response.Total = await fullResult.CountAsync();

            fullResult = fullResult.OrderByDescending(x => x.Created);
            if (records != null)
                fullResult = fullResult.Skip(page*records.Value)
                    .Take(records.Value);


            // Return the filtered list.
            response.List = await fullResult.ToListAsync();

            return response;
        }

        /// <summary>
        ///     Check whether two people are connected to each other or not.
        /// </summary>
        /// <param name="firstPerson"></param>
        /// <param name="secondPerson"></param>
        /// <returns></returns>
        public async Task<bool> IsPeopleConnected(int firstPerson, int secondPerson)
        {
            // Person is connect to him/herself.
            if (firstPerson == secondPerson)
                return true;

            var context = _dataContext.Context;

            // Take all people.
            IQueryable<Person> people = context.People;

            // Find the active people.
            people = people.Where(x => x.Status == (byte) StatusAccount.Active);

            // Find the 2 people in the list.
            people = people.Where(x => x.Id == firstPerson || x.Id == secondPerson);

            // Not only 2 people are returned. This means , one of 'em is disabled or both.
            var peopleCounter = await people.CountAsync();
            if (peopleCounter != 2)
                return false;

            // By default, take all relationships.
            IQueryable<Relation> relationships = context.Relations;

            // Find the relationship which these 2 people take part in.
            return await
                relationships.AnyAsync(
                    x =>
                        (x.Source == firstPerson && x.Target == secondPerson) ||
                        (x.Source == secondPerson && x.Target == firstPerson));
        }

        /// <summary>
        ///     Filter medical records by using specific conditions.
        /// </summary>
        /// <param name="relationships"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<Relation> FilterRelationships(
            IQueryable<Relation> relationships, FilterRelationshipViewModel filter)
        {
            // Base on requester role, do the filter.
            relationships = FilterRelationshipsByRequesterRole(relationships, filter);

            // Id is specified.
            if (filter.Id != null)
                relationships = relationships.Where(x => x.Id == filter.Id);

            // Created is specified.
            if (filter.MinCreated != null)
                relationships = relationships.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                relationships = relationships.Where(x => x.Created <= filter.MaxCreated);

            return relationships;
        }

        /// <summary>
        ///     Base on the requester role to do exact filter function.
        /// </summary>
        /// <param name="relationships"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<Relation> FilterRelationshipsByRequesterRole(
            IQueryable<Relation> relationships,
            FilterRelationshipViewModel filter)
        {
            // Requester is not defined.
            if (filter.Requester == null)
                throw new Exception("Requester must be specified.");

            // Patient is always the relationship request sender.
            if (filter.Requester.Role == (byte) Role.Patient)
            {
                // Find the source by using patient id.
                relationships = relationships.Where(x => x.Source == filter.Requester.Id);

                // Partner is specified.
                if (filter.Partner != null)
                    relationships = relationships.Where(x => x.Target == filter.Partner.Value);

                return relationships;
            }

            // Doctor is always the relationship request receiver.
            relationships = relationships.Where(x => x.Target == filter.Requester.Id);

            // Partner is specified. Find the source of relationship request.
            if (filter.Partner != null)
                relationships = relationships.Where(x => x.Source == filter.Partner.Value);

            return relationships;
        }

        #endregion
    }
}