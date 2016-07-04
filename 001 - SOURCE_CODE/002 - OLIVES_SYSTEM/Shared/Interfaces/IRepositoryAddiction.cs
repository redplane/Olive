using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryAddiction
    {
        /// <summary>
        ///     Filter allergies with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseAddictionFilter> FilterAddictionAsync(FilterAddictionViewModel filter);

        /// <summary>
        ///     Initialize an allergy with given information.
        /// </summary>
        /// <param name="info"></param>
        Task<Addiction> InitializeAddictionAsync(Addiction info);

        /// <summary>
        ///     Find allergy by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <param name="owner">Allergy owner</param>
        /// <returns></returns>
        Task<IList<Allergy>> FindAddictionAsync(int id, int owner);

        /// <summary>
        ///     Delete an allergy synchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeleteAddictionAsync(int id, int owner);
    }
}