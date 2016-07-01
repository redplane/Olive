using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositorySpecialty : IRepositorySpecialty
    {
        /// <summary>
        /// Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseSpecialtyFilter> FilterSpecialty(SpecialtyGetViewModel filter)
        {
            // Response initialization.
            var response = new ResponseSpecialtyFilter();

            // Initialize database connection context.
            var context = new OlivesHealthEntities();

            // Filtered results initialization.
            IQueryable<Specialty> results = context.Specialties;

            // Id has been specified.
            if (filter.Id != null)
                results = results.Where(x => x.Id == filter.Id);

            // Specialty name has been specified.
            if (!string.IsNullOrEmpty(filter.Name))
                results = results.Where(x => x.Name.Contains(filter.Name));

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

        /// <summary>
        /// Find specialty by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<Specialty>> FindSpecialty(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the specialty by using id.
            var result = context.Specialties.Where(x => x.Id == id);

            return await result.ToListAsync();
        }
    }
}