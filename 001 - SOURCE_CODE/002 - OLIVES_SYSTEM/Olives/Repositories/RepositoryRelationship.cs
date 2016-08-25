using System;
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
        public async Task<Relationship> FindRelationshipAsync(FilterRelationshipViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            // Take all relationships.
            IQueryable<Relationship> relationships = context.Relationships;

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

            // Using a transaction.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // By default, take all relationships.
                    IQueryable<Relationship> relationships = context.Relationships;

                    // Filter the relationships.
                    relationships = FilterRelationships(relationships, filter);

                    // Load the relationships into memory.
                    var relationshipsList = await relationships.ToListAsync();
                    
                    foreach (var relationship in relationshipsList)
                    {
                        #region Remove all appointments between these 2 people.

                        IQueryable<Appointment> appointments = context.Appointments;
                        appointments =
                            appointments.Where(
                                x =>
                                    (x.Maker == relationship.Source && x.Dater == relationship.Target) ||
                                    (x.Maker == relationship.Target && x.Dater == relationship.Source));

                        context.Appointments.RemoveRange(appointments);

                        #endregion

                        #region Medical record information change

                        IQueryable<MedicalRecord> medicalRecords = context.MedicalRecords;
                        medicalRecords =
                            medicalRecords.Where(
                                x =>
                                    (x.Owner == relationship.Source && x.Creator == relationship.Target) ||
                                    (x.Owner == relationship.Target && x.Creator == relationship.Source));

                        await medicalRecords.ForEachAsync(x =>
                        {
                            x.Creator = x.Owner;
                        });

                        #endregion

                        #region Prescription information change

                        IQueryable<Prescription> prescriptions = context.Prescriptions;
                        prescriptions =
                            prescriptions.Where(
                                x =>
                                    (x.Owner == relationship.Source && x.Creator == relationship.Target) ||
                                    (x.Owner == relationship.Target && x.Creator == relationship.Source));

                        await prescriptions.ForEachAsync(x =>
                        {
                            x.Creator = x.Owner;
                        });

                        #endregion

                        #region Experiment note

                        IQueryable<ExperimentNote> experimentNotes = context.ExperimentNotes;
                        experimentNotes =
                            experimentNotes.Where(
                                x =>
                                    (x.Owner == relationship.Source && x.Creator == relationship.Target) ||
                                    (x.Owner == relationship.Target && x.Creator == relationship.Source));

                        await experimentNotes.ForEachAsync(x =>
                        {
                            x.Creator = x.Owner;
                        });

                        #endregion

                        #region Medical note

                        IQueryable<MedicalNote> medicalNotes = context.MedicalNotes;
                        medicalNotes =
                            medicalNotes.Where(
                                x =>
                                    (x.Owner == relationship.Source && x.Creator == relationship.Target) ||
                                    (x.Owner == relationship.Target && x.Creator == relationship.Source));

                        await medicalNotes.ForEachAsync(x =>
                        {
                            x.Creator = x.Owner;
                        });

                        #endregion

                        #region Make all unseen notification be seen


                        IQueryable<Notification> notifications = context.Notifications;
                        notifications =
                            notifications.Where(
                                x =>
                                    (x.Broadcaster == relationship.Source && x.Recipient == relationship.Target) ||
                                    (x.Broadcaster == relationship.Target && x.Recipient == relationship.Source));

                        notifications = notifications.Where(x => !x.IsSeen);

                        await notifications.ForEachAsync(x =>
                        {
                            x.IsSeen = true;
                        });

                        #endregion
                    }

                    // Find the relation whose id is matched and has the specific person takes part in.
                    context.Relationships.RemoveRange(relationships);

                    // Save changes and retrieve the number of deleted records.
                    var records = await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return records;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        /// <summary>
        ///     Filter relationship base on the role of requester.
        /// </summary>
        /// <param name="filter"></param>
        public async Task<ResponseRelationshipFilter> FilterRelationshipAsync(FilterRelationshipViewModel filter)
        {
            var context = _dataContext.Context;

            // By default, take all relationship.
            IQueryable<Relationship> relationships = context.Relationships;
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
            IQueryable<Relationship> relationships = context.Relationships;

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
            IQueryable<Relationship> relationships = context.Relationships;

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
        private IQueryable<Relationship> FilterRelationships(
            IQueryable<Relationship> relationships, FilterRelationshipViewModel filter)
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
        private IQueryable<Relationship> FilterRelationshipsByRequesterRole(
            IQueryable<Relationship> relationships,
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