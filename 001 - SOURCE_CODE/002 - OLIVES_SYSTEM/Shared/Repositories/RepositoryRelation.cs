using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryRelation : IRepositoryRelation
    {
        /// <summary>
        ///     Initialize a relationship to database.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public async Task<Relation> InitializeRelationAsync(Relation relation)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Save relation to database.
            context.Relations.Add(relation);
            await context.SaveChangesAsync();

            return relation;
        }

        /// <summary>
        ///     Find a relation by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Relation> FindRelationshipAsync(int id, int? person, RoleRelationship? role,
            StatusRelation? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Query result.
            IQueryable<Relation> relationships = context.Relations;

            #region Query

            // Filter relationship by using id.
            relationships = relationships.Where(x => x.Id == id);

            // Source is specified.
            if (person != null)
            {
                // Role role is specified.
                if (role == RoleRelationship.Source)
                    relationships = relationships.Where(x => x.Source == person.Value);
                else if (role == RoleRelationship.Target)
                    relationships = relationships.Where(x => x.Target == person.Value);
                else
                    relationships = relationships.Where(x => x.Source == person.Value || x.Target == person.Value);
            }

            // Status is specified.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte)status);

            #endregion

            return await relationships.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Find the relation between 2 people.
        /// </summary>
        /// <param name="firstPerson"></param>
        /// <param name="secondPerson"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<IList<Relation>> FindRelationshipAsync(int firstPerson, int secondPerson, byte? status = null)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the participation of people in relationships.
            var results = context.Relations.Where(
                x =>
                    (x.Source == firstPerson && x.Target == secondPerson) || x.Source == secondPerson ||
                    x.Target == firstPerson);

            // Find the status which matches with the status we wanna find.
            results = results.Where(x => x.Status == status);
            return await results.ToListAsync();
        }

        /// <summary>
        ///     Find a relation whose id match with search condition and person is taking part in it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<IList<Relation>> FindRelationParticipation(int id, int person, byte? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationship.
            IQueryable<Relation> results = context.Relations;

            // Find the relation whose id is matched and has the specific person takes part in.
            results = results.Where(x => x.Id == id && (x.Source == id || x.Target == id));

            // Status is defined
            if (status != null)
                results = results.Where(x => x.Status == status);

            return await results.ToListAsync();
        }

        /// <summary>
        ///     Delete a relation asynchronously.
        /// </summary>
        /// <param name="id">Id of relationship</param>
        /// <param name="requester">Id of person who request to delete relationship.</param>
        /// <param name="role">The participation of requester in relationship.</param>
        /// <param name="status">Status of relationship.</param>
        /// <returns></returns>
        public async Task<int> DeleteRelationAsync(int id, int? requester, RoleRelationship? role,
            StatusRelation? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationships.
            IQueryable<Relation> relationships = context.Relations;

            // Find the relationship by using id.
            relationships = relationships.Where(x => x.Id == id);

            // Requester is defined. Find the his/her participation in the relationship.
            if (requester != null)
            {
                if (role == RoleRelationship.Source)
                    relationships = relationships.Where(x => x.Source == requester.Value);
                else if (role == RoleRelationship.Target)
                    relationships = relationships.Where(x => x.Target == requester.Value);
                else
                    relationships = relationships.Where(x => x.Source == requester || x.Target == requester);
            }

            // Status is defined.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte)status);

            // Find the relation whose id is matched and has the specific person takes part in.
            context.Relations.RemoveRange(relationships);

            return await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Filter relationship base on the role of requester.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="partner"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        public async Task<ResponseRelationshipFilter> FilterRelationshipAsync(int requester, int? partner,
            RoleRelationship? role, StatusRelation? status, int page, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationship.
            IQueryable<Relation> relationships = context.Relations;

            // In case the relationship role is defined.
            if (role == RoleRelationship.Source)
            {
                // Requester is the source of relationship.
                relationships = relationships.Where(x => x.Source == requester);

                // Therefore, partner is the target of relationship.
                if (partner != null)
                    relationships = relationships.Where(x => x.Target == partner.Value);
            }
            else if (role == RoleRelationship.Target)
            {
                // Requester is the target of relationship.
                relationships = relationships.Where(x => x.Target == requester);

                // Therefore, partner is the source of relationship.
                if (partner != null)
                    relationships = relationships.Where(x => x.Source == partner.Value);
            }
            else
                relationships = relationships.Where(x => x.Source == requester || x.Target == requester);

            // Status is defined.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte)status.Value);

            // Response initialization.
            var response = new ResponseRelationshipFilter();
            response.Total = await relationships.CountAsync();

            var skippedRecord = page * records;
            response.Relationships = await relationships
                .OrderByDescending(x => x.Created)
                .Skip(skippedRecord)
                .Take(records)
                .ToListAsync();

            return response;
        }

        /// <summary>
        ///     Filter related doctors.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<ResponseRelatedDoctorFilter> FilterRelatedDoctorAsync(int requester, StatusRelation? status,
            int page, int? records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationship.
            IQueryable<Relation> relationships = context.Relations;

            // Take the relationship whose source is requester and type is provide treatment.
            relationships = relationships.Where(x => x.Source == requester);

            // Status is defined.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte)status.Value);

            // Take all people who are doctor.
            IQueryable<Doctor> doctors = context.Doctors;

            var fullResult = from r in relationships
                             join d in doctors on r.Target equals d.Id
                             select new RelatedDoctorViewModel
                             {
                                 Doctor = d,
                                 RelationshipStatus = r.Status,
                                 Created = r.Created
                             };

            var response = new ResponseRelatedDoctorFilter();
            response.Total = await fullResult.CountAsync();

            fullResult = fullResult.OrderByDescending(x => x.Created);
            if (records != null)
                fullResult = fullResult.Skip(page * records.Value)
                    .Take(records.Value);


            // Return the filtered list.
            response.List = await fullResult.ToListAsync();

            return response;
        }

        /// <summary>
        /// Check whether two people are connected to each other or not.
        /// </summary>
        /// <param name="firstPerson"></param>
        /// <param name="secondPerson"></param>
        /// <returns></returns>
        public async Task<bool> IsPeopleConnected(int firstPerson, int secondPerson)
        {
            // Person is connect to him/herself.
            if (firstPerson == secondPerson)
                return true;

            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Take all people.
            IQueryable<Person> people = context.People;

            // Find the active people.
            people = people.Where(x => x.Status == (byte)StatusAccount.Active);

            // Find the 2 people in the list.
            people = people.Where(x => x.Id == firstPerson || x.Id == secondPerson);

            // Not only 2 people are returned. This means , one of 'em is disabled or both.
            var peopleCounter = await people.CountAsync();
            if (peopleCounter != 2)
                return false;
            
            // By default, take all relationships.
            IQueryable<Relation> relationships = context.Relations;

            // Filter to take active relationship only.
            relationships = relationships.Where(x => x.Status == (byte) StatusRelation.Active);

            // Find the relationship which these 2 people take part in.
            return await
                relationships.AnyAsync(
                    x =>
                        (x.Source == firstPerson && x.Target == secondPerson) ||
                        (x.Source == secondPerson && x.Target == firstPerson));
        }
    }
}