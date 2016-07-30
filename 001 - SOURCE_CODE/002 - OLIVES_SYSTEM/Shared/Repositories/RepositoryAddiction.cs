using System.Data.Entity;
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
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteAddictionAsync(FilterAddictionViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the record with the given conditions and remove 'em.
            var response = await FilterAddictionAsync(filter);

            // Delete and retrieve the affected records.
            context.Addictions.RemoveRange(response.Addictions);

            // Save changes.
            var records = await context.SaveChangesAsync();

            return records;
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

            // Id is specified.
            if (filter.Id != null)
                addictions = addictions.Where(x => x.Id == filter.Id);

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

            // Result filter
            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            addictions = addictions.OrderByDescending(x => x.Created);
                            break;
                        default:
                            addictions = addictions.OrderByDescending(x => x.LastModified);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            addictions = addictions.OrderBy(x => x.Created);
                            break;
                        default:
                            addictions = addictions.OrderBy(x => x.LastModified);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseAddictionFilter();

            // Count the number of matched records.
            response.Total = await addictions.CountAsync();

            // Result pagination.
            if (filter.Records != null)
            {
                addictions = addictions.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Take the paginated results.
            response.Addictions = addictions;

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