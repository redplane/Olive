using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryAllergy : IRepositoryAllergy
    {
        /// <summary>
        ///     Filter allergy by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseAllergyFilter> FilterAllergyAsync(FilterAllergyViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<Allergy> results = context.Allergies;

            #region Result filtering

            // Owner has been specified.
            if (filter.Owner != null)
                results = results.Where(x => x.Owner == filter.Owner);

            // Name has been specified.
            if (!string.IsNullOrEmpty(filter.Name))
                results = results.Where(x => x.Name.Contains(filter.Name));

            // Cause has been specified.
            if (!string.IsNullOrEmpty(filter.Cause))
                results = results.Where(x => x.Cause.Contains(filter.Cause));

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                results = results.Where(x => x.Note.Contains(filter.Note));

            // Either Min/Max Created has been specified.
            if (filter.MinCreated != null)
                results = results.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                results = results.Where(x => x.Created <= filter.MaxCreated);

            // Either Min/Max LastModified has been specified.
            if (filter.MinLastModified != null)
                results = results.Where(x => x.LastModified != null && x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                results = results.Where(x => x.LastModified != null && x.LastModified >= filter.MaxLastModified);

            #endregion

            // Initialize response.
            var response = new ResponseAllergyFilter();

            // Count the matched records before result truncation.
            response.Total = await results.CountAsync();
            
            // By default, order by last modified.
            results = results.OrderByDescending(x => x.LastModified);

            // Record is defined.
            if (filter.Records != null)
            {
                results = results.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            response.Allergies = await results
                .ToListAsync();

            return response;
        }

        /// <summary>
        ///     Initialize allergy to database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Allergy> InitializeAllergyAsync(Allergy info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add allergy to database context.
            context.Allergies.AddOrUpdate(info);

            // Submit allergy.
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        ///     Find allergy by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <param name="owner">Allergy owner</param>
        /// <returns></returns>
        public async Task<Allergy> FindAllergyAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all allergy.
            IQueryable<Allergy> allergies = context.Allergies;

            // Filter allergy by using id.
            allergies = allergies.Where(x => x.Id == id);

            // Ower is specified.
            if (owner != null)
                allergies = allergies.Where(x => x.Owner == owner);

            // Find allergy with given conditions.
            return await context.Allergies.FirstOrDefaultAsync(x => x.Id == id && x.Owner == owner);
        }

        /// <summary>
        ///     Delete an allergy synchrounously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<int> DeleteAllergyAsync(int id, int owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find and remove the condition matched result.
            context.Allergies.RemoveRange(context.Allergies.Where(x => x.Id == id && x.Owner == owner));

            // Count the number of affected records.
            var records = await context.SaveChangesAsync();
            return records;
        }
    }
}