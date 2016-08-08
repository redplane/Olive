using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces.Medical;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Repositories.Medical
{
    public class RepositoryMedicalNote : IRepositoryMedicalNote
    {
        /// <summary>
        ///     Find the medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MedicalNote> FindMedicalNoteAsync(int id)
        {
            // Database context initialization
            var context = new OlivesHealthEntities();

            // Find the medical note by using id.
            return await context.MedicalNotes.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Initialize a medical note asynchronously.
        /// </summary>
        /// <param name="medicalNote"></param>
        /// <returns></returns>
        public async Task<MedicalNote> InitializeMedicalNoteAsync(MedicalNote medicalNote)
        {
            // Database context initialization.draw
            var context = new OlivesHealthEntities();

            // Initialize/update medical notes.
            context.MedicalNotes.AddOrUpdate(medicalNote);

            // Save changes.
            await context.SaveChangesAsync();

            return medicalNote;
        }

        /// <summary>
        ///     Filter medical notes asynchronously by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalNoteFilter> FilterMedicalNotesAsync(FilterMedicalNoteViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all record by searching creator id.
            IQueryable<MedicalNote> medicalNotes = context.MedicalNotes;
            medicalNotes = FilterMedicalNotes(medicalNotes, filter);

            // Result sort.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case MedicalNoteFilterSort.Created:
                            medicalNotes = medicalNotes.OrderBy(x => x.Created);
                            break;
                        case MedicalNoteFilterSort.Note:
                            medicalNotes = medicalNotes.OrderBy(x => x.Note);
                            break;
                        case MedicalNoteFilterSort.Time:
                            medicalNotes = medicalNotes.OrderBy(x => x.Time);
                            break;
                        default:
                            medicalNotes = medicalNotes.OrderBy(x => x.LastModified);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case MedicalNoteFilterSort.Created:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.Created);
                            break;
                        case MedicalNoteFilterSort.Note:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.Note);
                            break;
                        case MedicalNoteFilterSort.Time:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.Time);
                            break;
                        default:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.LastModified);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseMedicalNoteFilter();

            // Calculate the total matched records.
            response.Total = await medicalNotes.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                medicalNotes = medicalNotes.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Truncate the results.
            response.MedicalNotes = await medicalNotes
                .ToListAsync();

            return response;
        }

        /// <summary>
        /// Filter medical notes by using specific conditions.
        /// </summary>
        /// <param name="medicalNotes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<MedicalNote> FilterMedicalNotes(IQueryable<MedicalNote> medicalNotes,
            FilterMedicalNoteViewModel filter)
        {
            // Id is defined.
            if (filter.Id != null)
                medicalNotes = medicalNotes.Where(x => x.Id == filter.Id.Value);

            // Medical record is defined.
            if (filter.MedicalRecord != null)
                medicalNotes = medicalNotes.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                medicalNotes = medicalNotes.Where(x => x.Owner == filter.Requester);
                if (filter.Partner != null)
                    medicalNotes = medicalNotes.Where(x => x.Creator == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                medicalNotes = medicalNotes.Where(x => x.Creator == filter.Requester);
                if (filter.Partner != null)
                    medicalNotes = medicalNotes.Where(x => x.Owner == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    medicalNotes =
                        medicalNotes.Where(x => x.Creator == filter.Requester || x.Owner == filter.Requester);
                else
                    medicalNotes =
                        medicalNotes.Where(
                            x =>
                                (x.Creator == filter.Requester && x.Owner == filter.Partner.Value) ||
                                (x.Creator == filter.Partner.Value && x.Owner == filter.Requester));
            }

            // Note is specified.
            if (filter.Note != null)
                medicalNotes = medicalNotes.Where(x => x.Note.Contains(filter.Note));

            // Time is specified.
            if (filter.MinTime != null) medicalNotes = medicalNotes.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null) medicalNotes = medicalNotes.Where(x => x.Time <= filter.MaxTime);

            // Created is defined.
            if (filter.MinCreated != null) medicalNotes = medicalNotes.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) medicalNotes = medicalNotes.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is defined.
            if (filter.MinLastModified != null)
                medicalNotes = medicalNotes.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                medicalNotes = medicalNotes.Where(x => x.LastModified <= filter.MaxLastModified);

            return medicalNotes;
        }

        /// <summary>
        /// Delete medical note by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteMedicalNoteAsync(FilterMedicalNoteViewModel filter)
        {
            var context = new OlivesHealthEntities();
            IQueryable<MedicalNote> medicalNotes = context.MedicalNotes;
            medicalNotes = FilterMedicalNotes(medicalNotes, filter);
            
            // Remove records.
            context.MedicalNotes.RemoveRange(medicalNotes);
            var records = await context.SaveChangesAsync();

            return records;
        }
    }
}