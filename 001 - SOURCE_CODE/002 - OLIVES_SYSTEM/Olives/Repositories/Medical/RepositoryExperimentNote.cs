using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Enumerations.Filter;
using Olives.Interfaces.Medical;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Repositories.Medical
{
    public class RepositoryExperimentNote : IRepositoryExperimentNote
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryExperimentNote(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find an experiment note asynchronously by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExperimentNote> FindExperimentNoteAsync(int id)
        {
            // Take all record first.
            var context = _dataContext.Context;
            IQueryable<ExperimentNote> experiments = context.ExperimentNotes;
            return await experiments.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Initialize experment note with information.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public async Task<ExperimentNote> InitializeExperimentNoteAsync(ExperimentNote note)
        {
            // Begin a transaction.
            var context = _dataContext.Context;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Initialize a note.
                    context.ExperimentNotes.AddOrUpdate(note);
                    await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return note;
                }
                catch
                {
                    // Exception occurs, rollback the transaction and throw the exception.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        ///     Delete experiment or its infos.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteExperimentNoteAsync(FilterExperimentNoteViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            IQueryable<ExperimentNote> experimentNotes = context.ExperimentNotes;
            experimentNotes = FilterExperimentNotes(experimentNotes, filter, context);
            context.ExperimentNotes.RemoveRange(experimentNotes);
            var records = await context.SaveChangesAsync();
            return records;
        }

        /// <summary>
        ///     Filter experiment note asynchronously by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseExperimentNoteFilter> FilterExperimentNoteAsync(FilterExperimentNoteViewModel filter)
        {
            // By default, take all experiment notes.
            var context = _dataContext.Context;
            IQueryable<ExperimentNote> experimentNotes = context.ExperimentNotes;
            experimentNotes = FilterExperimentNotes(experimentNotes, filter, context);

            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case ExperimentFilterSort.Created:
                            experimentNotes = experimentNotes.OrderBy(x => x.Created);
                            break;
                        case ExperimentFilterSort.LastModified:
                            experimentNotes = experimentNotes.OrderBy(x => x.LastModified);
                            break;
                        default:
                            experimentNotes = experimentNotes.OrderBy(x => x.Name);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case ExperimentFilterSort.Created:
                            experimentNotes = experimentNotes.OrderByDescending(x => x.Created);
                            break;
                        case ExperimentFilterSort.LastModified:
                            experimentNotes = experimentNotes.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            experimentNotes = experimentNotes.OrderByDescending(x => x.Name);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseExperimentNoteFilter();
            response.Total = await experimentNotes.CountAsync();

            // Record is defined, pagination must be done.
            if (filter.Records != null)
            {
                experimentNotes = experimentNotes.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Take the records.
            response.ExperimentNotes = await experimentNotes.ToListAsync();

            return response;
        }

        /// <summary>
        ///     Filter experiment notes by using conditions.
        /// </summary>
        /// <param name="experimentNotes"></param>
        /// <param name="filter"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private IQueryable<ExperimentNote> FilterExperimentNotes(IQueryable<ExperimentNote> experimentNotes,
            FilterExperimentNoteViewModel filter, OlivesHealthEntities context)
        {
            // Base on the requester role to do the filter function.
            experimentNotes = FilterExperimentNotesByRequesterRole(experimentNotes, filter, context);

            // Id is specified
            if (filter.Id != null)
                experimentNotes = experimentNotes.Where(x => x.Id == filter.Id.Value);

            // Filter by medical record id.
            if (filter.MedicalRecord != null)
                experimentNotes = experimentNotes.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            // Filter by medical record name.
            if (!string.IsNullOrWhiteSpace(filter.Name))
                experimentNotes = experimentNotes.Where(x => x.Name.Contains(filter.Name));

            // Time is defined.
            if (filter.MinTime != null)
                experimentNotes = experimentNotes.Where(x => x.Time >= filter.MinTime.Value);
            if (filter.MaxTime != null)
                experimentNotes = experimentNotes.Where(x => x.Time <= filter.MaxTime.Value);

            // Created is defined.
            if (filter.MinCreated != null)
                experimentNotes = experimentNotes.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null)
                experimentNotes = experimentNotes.Where(x => x.Created <= filter.MaxCreated.Value);

            // Last modified is defined.
            if (filter.MinLastModified != null)
                experimentNotes = experimentNotes.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                experimentNotes = experimentNotes.Where(x => x.LastModified <= filter.MaxLastModified);

            return experimentNotes;
        }

        /// <summary>
        ///     Base on the requester role to do exact filter function.
        /// </summary>
        /// <param name="experimentNotes"></param>
        /// <param name="filter"></param>
        /// <param name="olivesHealthEntities"></param>
        /// <returns></returns>
        private IQueryable<ExperimentNote> FilterExperimentNotesByRequesterRole(
            IQueryable<ExperimentNote> experimentNotes,
            FilterExperimentNoteViewModel filter, OlivesHealthEntities olivesHealthEntities)
        {
            // Requester is not defined.
            if (filter.Requester == null)
                throw new Exception("Requester must be specified.");

            // Patient only can see his/her records.
            if (filter.Requester.Role == (byte) Role.Patient)
            {
                experimentNotes = experimentNotes.Where(x => x.Owner == filter.Requester.Id);
                if (filter.Partner != null)
                    experimentNotes = experimentNotes.Where(x => x.Creator == filter.Partner.Value);

                return experimentNotes;
            }

            // Doctor can see every record whose owner has connection to him/her.
            IQueryable<Relationship> relationships = olivesHealthEntities.Relationships;
            relationships = relationships.Where(x => x.Target == filter.Requester.Id);

            // Partner is specified. This means to be a patient
            // Only patient can send request to doctor, that means he/she is the source of relationship.
            if (filter.Partner != null)
                relationships = relationships.Where(x => x.Source == filter.Partner.Value);

            var results = from r in relationships
                from e in experimentNotes
                where r.Source == e.Owner || r.Source == e.Creator
                select e;

            return results;
        }

        #endregion
    }
}