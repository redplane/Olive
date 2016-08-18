using System.Threading.Tasks;
using Olives.ViewModels.Filter.Personal;
using Olives.ViewModels.Response.Personal;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.PersonalNote
{
    public interface IRepositoryBloodPressure
    {
        /// <summary>
        ///     Initialize and save heart
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<BloodPressure> InitializeBloodPressureAsync(BloodPressure info);

        /// <summary>
        ///     Find heartbeat by using heartbeat note id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BloodPressure> FindBloodPressureAsync(int id);

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseBloodPressureFilter> FilterBloodPressureAsync(FilterBloodPressureViewModel filter);

        /// <summary>
        ///     Delete a heartbeat note asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        Task<int> DeleteBloodPressureAsync(FilterBloodPressureViewModel filter);
    }
}