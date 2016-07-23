using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryExperimentNote : IRepositoryExperimentNote
    {
        /// <summary>
        ///     Find an experiment note asynchronously by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExperimentNote> FindExperimentNoteAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Take all record first.
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
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Begin a transaction.
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
        /// <param name="experimentId"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<int> DeleteExperimentNotesAsync(int experimentId, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // By default, take all experiments.
                    IQueryable<ExperimentNote> experiments = context.ExperimentNotes;
                    experiments = experiments.Where(x => x.Id == experimentId);

                    // Owner is specified.
                    if (owner != null)
                        experiments = experiments.Where(x => x.Owner == owner.Value);

                    // Retrieve all experiment infos.
                    context.ExperimentNotes.RemoveRange(experiments);

                    // Save changes and calculate the number of affected record.
                    var records = await context.SaveChangesAsync();
                    transaction.Commit();

                    return records;
                }
                catch
                {
                    // As the exception happens, transaction should rollback.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Filter experiment note asynchronously by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseExperimentNoteFilter> FilterExperimentNotesAsync(FilterExperimentNoteViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all experiment notes.
            IQueryable<ExperimentNote> experiments = context.ExperimentNotes;

            // Filter by medical record id.
            experiments = experiments.Where(x => x.Id == filter.MedicalRecord);

            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                experiments = experiments.Where(x => x.Owner == filter.Requester);
                if (filter.Partner != null)
                    experiments = experiments.Where(x => x.Creator == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                experiments = experiments.Where(x => x.Creator == filter.Requester);
                if (filter.Partner != null)
                    experiments = experiments.Where(x => x.Owner == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    experiments =
                        experiments.Where(x => x.Creator == filter.Requester || x.Owner == filter.Requester);
                else
                    experiments =
                        experiments.Where(
                            x =>
                                (x.Creator == filter.Requester && x.Owner == filter.Partner.Value) ||
                                (x.Creator == filter.Partner.Value && x.Owner == filter.Requester));
            }

            // Filter by medical record name.
            if (!string.IsNullOrWhiteSpace(filter.Name))
                experiments = experiments.Where(x => x.Name.Contains(filter.Name));

            // Created is defined.
            if (filter.MinCreated != null) experiments = experiments.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null) experiments = experiments.Where(x => x.Created <= filter.MaxCreated.Value);

            // Last modified is defined.
            if (filter.MinLastModified != null)
                experiments = experiments.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                experiments = experiments.Where(x => x.LastModified <= filter.MaxLastModified);

            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case ExperimentFilterSort.Created:
                            experiments = experiments.OrderBy(x => x.Created);
                            break;
                        case ExperimentFilterSort.LastModified:
                            experiments = experiments.OrderBy(x => x.LastModified);
                            break;
                        default:
                            experiments = experiments.OrderBy(x => x.Name);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case ExperimentFilterSort.Created:
                            experiments = experiments.OrderByDescending(x => x.Created);
                            break;
                        case ExperimentFilterSort.LastModified:
                            experiments = experiments.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            experiments = experiments.OrderByDescending(x => x.Name);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseExperimentNoteFilter();
            response.Total = await experiments.CountAsync();

            // Record is defined, pagination must be done.
            if (filter.Records != null)
            {
                experiments = experiments.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }
            
            // Take the records.
            response.ExperimentNotes = await experiments.ToListAsync();

            return response;
        }
    }
}