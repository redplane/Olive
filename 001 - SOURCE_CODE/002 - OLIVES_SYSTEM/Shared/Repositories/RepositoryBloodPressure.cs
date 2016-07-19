using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryBloodPressure : IRepositoryBloodPressure
    {
        /// <summary>
        ///     Initialize a blood pressure note to database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<BloodPressure> InitializeBloodPressureNoteAsync(BloodPressure info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add allergy to database context.
            context.BloodPressures.AddOrUpdate(info);

            // Submit allergy.
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        ///     Find heartbeat note by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <param name="owner">Allergy owner</param>
        /// <returns></returns>
        public async Task<IList<BloodPressure>> FindBloodPressureNoteAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find heartbeat note by using id.
            var results = context.BloodPressures.Where(x => x.Id == id);

            // Owner has been specified.
            if (owner != null)
                results = results.Where(x => x.Owner == owner);

            return await results.ToListAsync();
        }

        /// <summary>
        ///     Delete a blood pressure note asynchrounously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<int> DeleteBloodPressureNoteAsync(int id, int owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find and delete a note.
            context.BloodPressures.RemoveRange(context.BloodPressures.Where(x => x.Id == id && x.Owner == owner));

            // Count the affeted records.
            var deletedRecords = await context.SaveChangesAsync();
            return deletedRecords;
        }

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseBloodPressureFilter> FilterBloodPressureNoteAsync(FilterBloodPressureViewModel filter)
        {
            // Data context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all information.
            IQueryable<BloodPressure> results = context.BloodPressures;

            // Owner has been specified.
            if (filter.Owner != null)
                results = results.Where(x => x.Owner == filter.Owner);

            // Systolic has been specified.
            if (filter.MinSystolic != null)
                results = results.Where(x => x.Systolic >= filter.MinSystolic);
            if (filter.MaxSystolic != null)
                results = results.Where(x => x.Systolic <= filter.MaxSystolic);

            // Diastolic has been specified.
            if (filter.MinDiastolic != null)
                results = results.Where(x => x.Diastolic >= filter.MinDiastolic);
            if (filter.MaxDiastolic != null)
                results = results.Where(x => x.Diastolic <= filter.MaxDiastolic);


            // Time has been specified.
            if (filter.MinTime != null)
                results = results.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null)
                results = results.Where(x => x.Time <= filter.MaxTime);

            // Created has been specified.
            if (filter.MinCreated != null)
                results = results.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                results = results.Where(x => x.Created <= filter.MaxCreated);

            // LastModified has been specified.
            if (filter.MinLastModified != null)
                results = results.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                results = results.Where(x => x.LastModified <= filter.MaxLastModified);

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                results = results.Where(x => x.Note.Contains(filter.Note));

            // Order by last modified.
            results = results.OrderByDescending(x => x.LastModified);

            // Initialize response and throw result back.
            var response = new ResponseBloodPressureFilter();
            response.Total = await results.CountAsync();

            // Calculate what records should be shown up.
            var skippedRecords = filter.Page*filter.Records;

            // Sort the result.
            switch (filter.Sort)
            {
                case NoteResultSort.Created:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        results = results.OrderBy(x => x.Created);
                        break;
                    }

                    results = results.OrderByDescending(x => x.Created);
                    break;
                case NoteResultSort.LastModified:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        results = results.OrderBy(x => x.LastModified);
                        break;
                    }
                    results = results.OrderByDescending(x => x.LastModified);
                    break;
                default:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        results = results.OrderBy(x => x.Time);
                        break;
                    }

                    results = results.OrderByDescending(x => x.Time);
                    break;
            }

            // Record is defined.
            if (filter.Records != null)
            {
                results = results.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            response.BloodPressures = await results
                .Select(x => new BloodPressureViewModel
                {
                    Id = x.Id,
                    Systolic = x.Systolic,
                    Diastolic = x.Diastolic,
                    Time = x.Time,
                    Note = x.Note,
                    Created = x.Created,
                    LastModified = x.LastModified
                })
                .ToListAsync();

            // Return filtered result.
            return response;
        }
    }
}