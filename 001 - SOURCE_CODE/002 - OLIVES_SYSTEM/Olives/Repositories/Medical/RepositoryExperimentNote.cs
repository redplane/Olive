using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces.Medical;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

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
        public async Task<ExperimentNote> InitializeExperimentNote(ExperimentNote note)
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
        public async Task<int> DeleteExperimentNotesAsync(FilterExperimentNoteViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;
            IQueryable<ExperimentNote> experimentNotes = context.ExperimentNotes;
            context.ExperimentNotes.RemoveRange(experimentNotes);
            var records = await context.SaveChangesAsync();
            return records;
        }

        /// <summary>
        ///     Filter experiment note asynchronously by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseExperimentNoteFilter> FilterExperimentNotesAsync(FilterExperimentNoteViewModel filter)
        {
            // By default, take all experiment notes.
            var context = _dataContext.Context;
            IQueryable<ExperimentNote> experimentNotes = context.ExperimentNotes;
            experimentNotes = FilterExperimentNotes(experimentNotes, filter);

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
        /// <returns></returns>
        public IQueryable<ExperimentNote> FilterExperimentNotes(IQueryable<ExperimentNote> experimentNotes,
            FilterExperimentNoteViewModel filter)
        {
            // Id is specified
            if (filter.Id != null)
                experimentNotes = experimentNotes.Where(x => x.Id == filter.Id.Value);

            // Filter by medical record id.
            if (filter.MedicalRecord != null)
                experimentNotes = experimentNotes.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                experimentNotes = experimentNotes.Where(x => x.Owner == filter.Requester);
                if (filter.Partner != null)
                    experimentNotes = experimentNotes.Where(x => x.Creator == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                experimentNotes = experimentNotes.Where(x => x.Creator == filter.Requester);
                if (filter.Partner != null)
                    experimentNotes = experimentNotes.Where(x => x.Owner == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    experimentNotes =
                        experimentNotes.Where(x => x.Creator == filter.Requester || x.Owner == filter.Requester);
                else
                    experimentNotes =
                        experimentNotes.Where(
                            x =>
                                (x.Creator == filter.Requester && x.Owner == filter.Partner.Value) ||
                                (x.Creator == filter.Partner.Value && x.Owner == filter.Requester));
            }

            // Filter by medical record name.
            if (!string.IsNullOrWhiteSpace(filter.Name))
                experimentNotes = experimentNotes.Where(x => x.Name.Contains(filter.Name));

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

        #endregion
    }
}