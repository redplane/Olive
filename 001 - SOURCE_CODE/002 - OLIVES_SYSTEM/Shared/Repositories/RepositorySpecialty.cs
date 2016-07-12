using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositorySpecialty : IRepositorySpecialty
    {
        /// <summary>
        ///     Find the specialty by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Specialty> FindSpecialtyAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the specialty by using id.
            IQueryable<Specialty> specialties = context.Specialties;
            return await specialties.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseSpecialtyFilter> FilterSpecialtyAsync(FilterSpecialtyViewModel filter)
        {
            // Response initialization.
            var response = new ResponseSpecialtyFilter();

            // Initialize database connection context.
            var context = new OlivesHealthEntities();

            // Filtered results initialization.
            IQueryable<Specialty> results = context.Specialties;

            // Specialty name has been specified.
            if (!string.IsNullOrEmpty(filter.Name))
                results = results.Where(x => x.Name.Contains(filter.Name));

            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    results = results.OrderBy(x => x.Name);
                    break;
                default:
                    results = results.OrderByDescending(x => x.Name);
                    break;
            }

            // Count the matching results.
            response.Total = await results.CountAsync();

            // Finally, do the pagination.
            var skippedRecords = filter.Page*filter.Records;
            results = results
                .OrderBy(x => x.Name)
                .Skip(skippedRecords)
                .Take(filter.Records);

            // Retrieve the specialty list.
            response.Specialties = await results.ToListAsync();
            return response;
        }
    }
}