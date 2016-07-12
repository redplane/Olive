using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositorySugarblood
    {
        /// <summary>
        ///     Initialize and save heart
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<SugarBlood> InitializeSugarbloodNoteAsync(SugarBlood info);

        /// <summary>
        ///     Find heartbeat by using heartbeat note id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<IList<SugarBlood>> FindSugarbloodNoteAsync(int id, int? owner);

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseSugarbloodFilter> FilterSugarbloodNoteAsync(FilterSugarbloodViewModel filter);

        /// <summary>
        ///     Delete a heartbeat note asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        Task<int> DeleteSugarbloodNoteAsync(int id, int owner);
    }
}