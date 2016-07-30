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
    public class RepositoryBloodSugar : IRepositoryBloodSugar
    {
        /// <summary>
        ///     Initialize sugarblood note to database.
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
        ///     Find sugarblood note by using id and owner id.
        /// </summary>
        /// <param name="id">Blood sugar Id</param>
        /// <returns></returns>
        public async Task<SugarBlood> FindBloodSugarAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            return await context.SugarBloods.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseBloodSugarFilter> FilterBloodSugarAsync(FilterBloodSugarViewModel filter)
        {
            // Data context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all information.
            IQueryable<SugarBlood> bloodSugars = context.SugarBloods;

            // Owner has been specified.
            if (filter.Owner != null)
                bloodSugars = bloodSugars.Where(x => x.Owner == filter.Owner);

            // Value has been specified.
            if (filter.MinValue != null)
                bloodSugars = bloodSugars.Where(x => x.Value >= filter.MinValue);
            if (filter.MinValue != null)
                bloodSugars = bloodSugars.Where(x => x.Value <= filter.MaxValue);

            // Time has been specified.
            if (filter.MinTime != null)
                bloodSugars = bloodSugars.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null)
                bloodSugars = bloodSugars.Where(x => x.Time <= filter.MaxTime);

            // Created has been specified.
            if (filter.MinCreated != null)
                bloodSugars = bloodSugars.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                bloodSugars = bloodSugars.Where(x => x.Created <= filter.MaxCreated);

            // LastModified has been specified.
            if (filter.MinLastModified != null)
                bloodSugars = bloodSugars.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                bloodSugars = bloodSugars.Where(x => x.LastModified <= filter.MaxLastModified);

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                bloodSugars = bloodSugars.Where(x => x.Note.Contains(filter.Note));

            // Order by last modified.
            bloodSugars = bloodSugars.OrderByDescending(x => x.LastModified);

            // Initialize response and throw result back.
            var response = new ResponseBloodSugarFilter();
            response.Total = await bloodSugars.CountAsync();

            // Calculate what records should be shown up.
            var skippedRecords = filter.Page*filter.Records;

            // Sort the result.
            switch (filter.Sort)
            {
                case NoteResultSort.Created:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        bloodSugars = bloodSugars.OrderBy(x => x.Created);
                        break;
                    }

                    bloodSugars = bloodSugars.OrderByDescending(x => x.Created);
                    break;
                case NoteResultSort.LastModified:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        bloodSugars = bloodSugars.OrderBy(x => x.LastModified);
                        break;
                    }
                    bloodSugars = bloodSugars.OrderByDescending(x => x.LastModified);
                    break;
                default:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        bloodSugars = bloodSugars.OrderBy(x => x.Time);
                        break;
                    }

                    bloodSugars = bloodSugars.OrderByDescending(x => x.Time);
                    break;
            }

            // Record is defined.
            if (filter.Records != null)
            {
                bloodSugars = bloodSugars.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            response.Sugarbloods = bloodSugars;

            // Return filtered result.
            return response;
        }

        /// <summary>
        ///     Delete a sugarblood note asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        public async Task<int> DeleteBloodSugarAsync(FilterBloodSugarViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Call the filter function.
            var result = await FilterBloodSugarAsync(filter);

            // Delete the filtered record.
            context.SugarBloods.RemoveRange(result.Sugarbloods);

            // Save changes and count the affected records.
            var records = await context.SaveChangesAsync();
            return records;
        }
    }
}