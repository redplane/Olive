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
    public class RepositorySugarblood : IRepositorySugarblood
    {
        /// <summary>
        /// Initialize sugarblood note to database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<SugarBlood> InitializeSugarbloodNoteAsync(SugarBlood info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add allergy to database context.
            context.SugarBloods.AddOrUpdate(info);

            // Submit allergy.
            await context.SaveChangesAsync();

            return info;
        }
        
        /// <summary>
        /// Find sugarblood note by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <param name="owner">Allergy owner</param>
        /// <returns></returns>
        public async Task<IList<SugarBlood>> FindSugarbloodNoteAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find heartbeat note by using id.
            var results = context.SugarBloods.Where(x => x.Id == id);

            // Owner has been specified.
            if (owner != null)
                results = results.Where(x => x.Owner == owner);

            return await results.ToListAsync();
        }
        
        /// <summary>
        /// Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseSugarbloodFilter> FilterSugarbloodNoteAsync(FilterSugarbloodViewModel filter)
        {
            // Data context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all information.
            IQueryable<SugarBlood> results = context.SugarBloods;

            // Owner has been specified.
            if (filter.Owner != null)
                results = results.Where(x => x.Owner == filter.Owner);

            // Value has been specified.
            if (filter.MinValue != null)
                results = results.Where(x => x.Value >= filter.MinValue);
            if (filter.MinValue != null)
                results = results.Where(x => x.Value <= filter.MaxValue);

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
            var response = new ResponseSugarbloodFilter();
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

            response.Sugarbloods = await results.Skip(skippedRecords)
                .Take(filter.Records)
                .Select(x => new SugarbloodViewModel()
                {
                    Id = x.Id,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    Note = x.Note,
                    Time = x.Time,
                    Value = x.Value
                })
                .ToListAsync();

            // Return filtered result.
            return response;
        }

        /// <summary>
        /// Delete a sugarblood note asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        public async Task<int> DeleteSugarbloodNoteAsync(int id, int owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Remove records by querying id and owner id.
            context.SugarBloods.RemoveRange(context.SugarBloods.Where(x => x.Id == id && x.Owner == owner));

            var deletedRecords = await context.SaveChangesAsync();
            await context.SaveChangesAsync();
            return deletedRecords;
        }
    }
}