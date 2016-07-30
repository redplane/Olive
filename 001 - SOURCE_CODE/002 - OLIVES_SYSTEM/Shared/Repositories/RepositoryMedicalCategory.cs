using System;
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
    public class RepositoryMedicalCategory : IRepositoryMedicalCategory
    {
        /// <summary>
        ///     Find medical category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public async Task<MedicalCategory> FindMedicalCategoryAsync(int? id, string name, StringComparison? comparison)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all record.
            IQueryable<MedicalCategory> medicalCategories = context.MedicalCategories;

            // Id is defined.
            if (id != null)
                medicalCategories = medicalCategories.Where(x => x.Id == id);

            // Name is defined.
            if (name != null)
                medicalCategories =
                    medicalCategories.Where(x => x.Name.Equals(name, comparison ?? StringComparison.Ordinal));

            return await medicalCategories.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Initialize medical category.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<MedicalCategory> InitializeMedicalCategoryAsync(MedicalCategory initializer)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize or update data.
            context.MedicalCategories.AddOrUpdate(initializer);

            // Save changes.
            await context.SaveChangesAsync();
            return initializer;
        }

        /// <summary>
        ///     Filter medical categories asynchrously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalCategoryFilter> FilterMedicalCategoryAsync(
            FilterMedicalCategoryViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all categories.
            IQueryable<MedicalCategory> categories = context.MedicalCategories;

            // Name is defined.
            if (filter.Name != null)
                categories = categories.Where(x => x.Name.Contains(filter.Name));

            // Created is defined.
            if (filter.MinCreated != null) categories = categories.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) categories = categories.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is defined.
            if (filter.MinLastModified != null) categories = categories.Where(x => x.LastModified >= filter.MinCreated);
            if (filter.MaxLastModified != null)
                categories = categories.Where(x => x.LastModified >= filter.MaxLastModified);

            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case MedicalCategoryFilterSort.Created:
                            categories = categories.OrderBy(x => x.Created);
                            break;
                        case MedicalCategoryFilterSort.LastModified:
                            categories = categories.OrderBy(x => x.LastModified);
                            break;
                        default:
                            categories = categories.OrderBy(x => x.Name);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case MedicalCategoryFilterSort.Created:
                            categories = categories.OrderByDescending(x => x.Created);
                            break;
                        case MedicalCategoryFilterSort.LastModified:
                            categories = categories.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            categories = categories.OrderByDescending(x => x.Name);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseMedicalCategoryFilter();

            // Count the total matched result.
            response.Total = await categories.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                categories = categories.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }
            // Do pagination.
            response.MedicalCategories = await categories
                .ToListAsync();

            return response;
        }
    }
}