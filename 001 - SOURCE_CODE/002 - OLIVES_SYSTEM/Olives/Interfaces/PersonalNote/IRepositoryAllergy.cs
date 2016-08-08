using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.PersonalNote
{
    public interface IRepositoryAllergy
    {
        /// <summary>
        ///     Filter allergies with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseAllergyFilter> FilterAllergyAsync(FilterAllergyViewModel filter);

        /// <summary>
        ///     Initialize an allergy with given information.
        /// </summary>
        /// <param name="info"></param>
        Task<Allergy> InitializeAllergyAsync(Allergy info);

        /// <summary>
        ///     Find allergy by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <returns></returns>
        Task<Allergy> FindAllergyAsync(int id);

        /// <summary>
        ///     Delete an allergy asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteAllergyAsync(FilterAllergyViewModel filter);
    }
}