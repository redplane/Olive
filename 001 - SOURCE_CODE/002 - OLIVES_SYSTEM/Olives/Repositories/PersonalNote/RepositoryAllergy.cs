using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces.PersonalNote;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Repositories.PersonalNote
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
            IQueryable<Allergy> allergies = context.Allergies;
            allergies = FilterAllergiesAsync(allergies, filter);
            
            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            allergies = allergies.OrderByDescending(x => x.Created);
                            break;
                        default:
                            allergies = allergies.OrderByDescending(x => x.LastModified);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            allergies = allergies.OrderBy(x => x.Created);
                            break;
                        default:
                            allergies = allergies.OrderBy(x => x.LastModified);
                            break;
                    }
                    break;
            }

            // Initialize response.
            var response = new ResponseAllergyFilter();

            // Count the matched records before result truncation.
            response.Total = await allergies.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                allergies = allergies.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            response.Allergies = allergies;
            return response;
        }

        /// <summary>
        /// Filter allergies by using specific conditions.
        /// </summary>
        /// <param name="allergies"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IQueryable<Allergy> FilterAllergiesAsync(IQueryable<Allergy> allergies, FilterAllergyViewModel filter)
        {
            // Id is specified.
            if (filter.Id != null)
                allergies = allergies.Where(x => x.Id == filter.Id.Value);

            // Owner has been specified.
            if (filter.Owner != null)
                allergies = allergies.Where(x => x.Owner == filter.Owner);

            // Name has been specified.
            if (!string.IsNullOrEmpty(filter.Name))
                allergies = allergies.Where(x => x.Name.Contains(filter.Name));

            // Cause has been specified.
            if (!string.IsNullOrEmpty(filter.Cause))
                allergies = allergies.Where(x => x.Cause.Contains(filter.Cause));

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                allergies = allergies.Where(x => x.Note.Contains(filter.Note));

            // Either Min/Max Created has been specified.
            if (filter.MinCreated != null)
                allergies = allergies.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                allergies = allergies.Where(x => x.Created <= filter.MaxCreated);

            // Either Min/Max LastModified has been specified.
            if (filter.MinLastModified != null)
                allergies = allergies.Where(x => x.LastModified != null && x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                allergies = allergies.Where(x => x.LastModified != null && x.LastModified >= filter.MaxLastModified);

            return allergies;
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
        /// <returns></returns>
        public async Task<Allergy> FindAllergyAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            return await context.Allergies.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Delete an allergy synchrounously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteAllergyAsync(FilterAllergyViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<Allergy> allergies = context.Allergies;
            
            // Do filter.
            allergies = FilterAllergiesAsync(allergies, filter);

            // Remove the filtered result.
            context.Allergies.RemoveRange(allergies);

            // Save change.
            var records = await context.SaveChangesAsync();

            return records;
        }
    }
}