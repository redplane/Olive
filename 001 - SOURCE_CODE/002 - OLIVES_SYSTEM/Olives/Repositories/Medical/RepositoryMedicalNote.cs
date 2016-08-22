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
    public class RepositoryMedicalNote : IRepositoryMedicalNote
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructor

        public RepositoryMedicalNote(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find the medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MedicalNote> FindMedicalNoteAsync(int id)
        {
            // Find the medical note by using id.
            var context = _dataContext.Context;
            return await context.MedicalNotes.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Initialize a medical note asynchronously.
        /// </summary>
        /// <param name="medicalNote"></param>
        /// <returns></returns>
        public async Task<MedicalNote> InitializeMedicalNoteAsync(MedicalNote medicalNote)
        {
            // Initialize/update medical notes.
            var context = _dataContext.Context;
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
        public async Task<ResponseMedicalNoteFilter> FilterMedicalNoteAsync(FilterMedicalNoteViewModel filter)
        {
            // By default, take all record by searching creator id.
            var context = _dataContext.Context;
            IQueryable<MedicalNote> medicalNotes = context.MedicalNotes;

            // Do the basic filter.
            medicalNotes = FilterMedicalNotes(medicalNotes, filter, context);

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
        ///     Filter medical notes by using specific conditions.
        /// </summary>
        /// <param name="medicalNotes"></param>
        /// <param name="filter"></param>
        /// <param name="olivesHealthEntities"></param>
        /// <returns></returns>
        private IQueryable<MedicalNote> FilterMedicalNotes(IQueryable<MedicalNote> medicalNotes,
            FilterMedicalNoteViewModel filter, OlivesHealthEntities olivesHealthEntities)
        {
            // Base on requester role to do filter.
            medicalNotes = FilterMedicalNotesByRequesterRole(medicalNotes, filter, olivesHealthEntities);

            // Id is defined.
            if (filter.Id != null)
                medicalNotes = medicalNotes.Where(x => x.Id == filter.Id.Value);

            // Medical record is defined.
            if (filter.MedicalRecord != null)
                medicalNotes = medicalNotes.Where(x => x.MedicalRecordId == filter.MedicalRecord);

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
        ///     Base on the requester role to do exact filter function.
        /// </summary>
        /// <param name="medicalNotes"></param>
        /// <param name="filter"></param>
        /// <param name="olivesHealthEntities"></param>
        /// <returns></returns>
        private IQueryable<MedicalNote> FilterMedicalNotesByRequesterRole(IQueryable<MedicalNote> medicalNotes,
            FilterMedicalNoteViewModel filter, OlivesHealthEntities olivesHealthEntities)
        {
            // Requester is not defined.
            if (filter.Requester == null)
                throw new Exception("Requester must be specified.");

            // Patient only can see his/her records.
            if (filter.Requester.Role == (byte) Role.Patient)
            {
                medicalNotes = medicalNotes.Where(x => x.Owner == filter.Requester.Id);
                if (filter.Partner != null)
                    medicalNotes = medicalNotes.Where(x => x.Creator == filter.Partner.Value);

                return medicalNotes;
            }

            // Doctor can see every record whose owner has connection to him/her.
            IQueryable<Relationship> relationships = olivesHealthEntities.Relationships;
            relationships = relationships.Where(x => x.Target == filter.Requester.Id);

            // Partner is specified. This means to be a patient
            // Only patient can send request to doctor, that means he/she is the source of relationship.
            if (filter.Partner != null)
                relationships = relationships.Where(x => x.Source == filter.Partner.Value);

            var results = from r in relationships
                from m in medicalNotes
                where r.Source == m.Owner || r.Source == m.Creator
                select m;

            return results;
        }

        /// <summary>
        ///     Delete medical note by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteMedicalNoteAsync(FilterMedicalNoteViewModel filter)
        {
            var context = _dataContext.Context;
            IQueryable<MedicalNote> medicalNotes = context.MedicalNotes;
            medicalNotes = FilterMedicalNotes(medicalNotes, filter, context);

            // Remove records.
            context.MedicalNotes.RemoveRange(medicalNotes);
            var records = await context.SaveChangesAsync();

            return records;
        }

        #endregion
    }
}