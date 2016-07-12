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
        ///     Find an addiction by using id.
        /// </summary>
        /// <param name="id">Addiction Id</param>
        /// <returns></returns>
        Task<Addiction> FindAddictionAsync(int id);

        /// <summary>
        ///     Delete an allergy synchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeleteAddictionAsync(int id, int owner);
    }
}