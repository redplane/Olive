using System;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryMedicalCategory
    {
        /// <summary>
        /// Find medical category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        Task<MedicalCategory> FindMedicalCategoryAsync(int? id, string name, StringComparison? comparison);

        /// <summary>
        /// Initialize medical category.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<MedicalCategory> InitializeMedicalCategoryAsync(MedicalCategory initializer);

        /// <summary>
        /// Filter medical categories asynchrously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalCategoryFilter> FilterMedicalCategoryAsync(FilterMedicalCategoryViewModel filter);
    }
}