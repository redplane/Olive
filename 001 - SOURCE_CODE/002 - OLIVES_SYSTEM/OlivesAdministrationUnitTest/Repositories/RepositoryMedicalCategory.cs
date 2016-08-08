using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace OlivesAdministration.Test.Repositories
{
    public class RepositoryMedicalCategory : IRepositoryMedicalCategory
    {
        #region Properties

        /// <summary>
        ///     List of medical categories which is used for testing purpose.
        /// </summary>
        public List<MedicalCategory> MedicalCategories { get; set; }

        #endregion
        
        /// <summary>
        ///     Find medical category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public async Task<MedicalCategory> FindMedicalCategoryAsync(int? id, string name, StringComparison? comparison)
        {
            // By default, take all record.
            IQueryable<MedicalCategory> medicalCategories = new EnumerableQuery<MedicalCategory>(MedicalCategories);

            // Id is defined.
            if (id != null)
                medicalCategories = medicalCategories.Where(x => x.Id == id);

            // Name is defined.
            if (name != null)
                medicalCategories =
                    medicalCategories.Where(x => x.Name.Equals(name, comparison ?? StringComparison.Ordinal));

            return medicalCategories.FirstOrDefault();
        }

        /// <summary>
        ///     Find medical category synchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="comparision"></param>
        /// <returns></returns>
        public MedicalCategory FindMedicalCategory(int? id, string name, StringComparison? comparision)
        {
            // By default, take all record.
            IQueryable<MedicalCategory> medicalCategories = new EnumerableQuery<MedicalCategory>(MedicalCategories);

            // Id is defined.
            if (id != null)
                medicalCategories = medicalCategories.Where(x => x.Id == id);

            // Name is defined.
            if (name != null)
                medicalCategories =
                    medicalCategories.Where(x => x.Name.Equals(name, comparision ?? StringComparison.Ordinal));

            return medicalCategories.FirstOrDefault();
        }

        /// <summary>
        ///     Initialize medical category.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<MedicalCategory> InitializeMedicalCategoryAsync(MedicalCategory initializer)
        {
            // Database context initialization.
            IQueryable<MedicalCategory> medicalCategories = new EnumerableQuery<MedicalCategory>(MedicalCategories);

            if (initializer.Id == 0)
            {
                initializer.Id = MedicalCategories.Count;
                MedicalCategories.Add(initializer);
                return initializer;
            }

            var index = MedicalCategories.IndexOf(initializer);
            MedicalCategories[index] = initializer;

            // Save changes.
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
            // By default, take all categories.
            IQueryable<MedicalCategory> categories = new EnumerableQuery<MedicalCategory>(MedicalCategories);

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
            response.Total = categories.Count();

            // Record is defined.
            if (filter.Records != null)
            {
                categories = categories.Skip(filter.Page * filter.Records.Value)
                    .Take(filter.Records.Value);
            }
            // Do pagination.
            response.MedicalCategories = categories
                .ToList();

            return response;
        }
    }
}