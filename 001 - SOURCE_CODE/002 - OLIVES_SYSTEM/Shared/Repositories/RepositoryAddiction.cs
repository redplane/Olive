﻿using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryAddiction : IRepositoryAddiction
    {
        /// <summary>
        ///     Delete an addiction asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<int> DeleteAddictionAsync(int id, int owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the record with the given conditions and remove 'em.
            context.Addictions.RemoveRange(context.Addictions.Where(x => x.Id == id && x.Owner == owner));
            var affectedRecords = await context.SaveChangesAsync();
            return affectedRecords;
        }

        /// <summary>
        ///     Filter a list of addiction with the specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseAddictionFilter> FilterAddictionAsync(FilterAddictionViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records from database.
            IQueryable<Addiction> addictions = context.Addictions;

            // Owner has been specified.
            if (filter.Owner != null)
                addictions = addictions.Where(x => x.Owner == filter.Owner);

            // Cause has been specified.
            if (!string.IsNullOrWhiteSpace(filter.Cause))
                addictions = addictions.Where(x => x.Cause.Contains(filter.Cause));

            // Note has been specified.
            if (!string.IsNullOrWhiteSpace(filter.Note))
                addictions = addictions.Where(x => x.Note.Contains(filter.Note));

            // Created has been specified.
            if (filter.MinCreated != null) addictions = addictions.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) addictions = addictions.Where(x => x.Created <= filter.MaxCreated);

            // Last modified has been specified.
            if (filter.MinLastModified != null)
                addictions = addictions.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                addictions = addictions.Where(x => x.LastModified <= filter.MaxLastModified);

            // Calculate the records should be skipped.
            var skippedRecords = filter.Page*filter.Records;

            // Result sorting.
            switch (filter.Sort)
            {
                case NoteResultSort.Created:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        addictions = addictions.OrderBy(x => x.Created);
                        break;
                    }
                    addictions = addictions.OrderByDescending(x => x.Created);
                    break;
                default:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        addictions = addictions.OrderBy(x => x.LastModified);
                        break;
                    }
                    addictions = addictions.OrderByDescending(x => x.LastModified);
                    break;
            }

            // Response initialization.
            var response = new ResponseAddictionFilter();
            
            // Count the number of matched records.
            response.Total = await addictions.CountAsync();

            if (filter.Records != null)
            {
                addictions = addictions.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }
            response.Addictions = await addictions
                .ToListAsync();

            return response;
        }

        /// <summary>
        ///     Find an addiction by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Addiction> FindAddictionAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the first matched addiction with id.
            var result = await context.Addictions.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        /// <summary>
        ///     Initialize an addiction asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Addiction> InitializeAddictionAsync(Addiction info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Insert the source record to database.
            context.Addictions.AddOrUpdate(info);
            await context.SaveChangesAsync();
            return info;
        }
    }
}