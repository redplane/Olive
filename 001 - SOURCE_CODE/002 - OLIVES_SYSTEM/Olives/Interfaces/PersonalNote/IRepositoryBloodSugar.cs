using System.Threading.Tasks;
using Olives.ViewModels.Filter.Personal;
using Olives.ViewModels.Response.Personal;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.PersonalNote
{
    public interface IRepositoryBloodSugar
    {
        /// <summary>
        ///     Initialize and save blood sugar
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<SugarBlood> InitializeSugarbloodNoteAsync(SugarBlood info);

        /// <summary>
        ///     Find blood sugar by using blood sugar note id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SugarBlood> FindBloodSugarAsync(int id);

        /// <summary>
        ///     Find blood sugar by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseBloodSugarFilter> FilterBloodSugarAsync(FilterBloodSugarViewModel filter);

        /// <summary>
        ///     Delete a blood sugar note asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        Task<int> DeleteBloodSugarAsync(FilterBloodSugarViewModel filter);
    }
}