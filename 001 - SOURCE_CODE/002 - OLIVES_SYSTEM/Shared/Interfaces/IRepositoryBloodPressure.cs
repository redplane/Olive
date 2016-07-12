using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryBloodPressure
    {
        /// <summary>
        ///     Initialize and save heart
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<BloodPressure> InitializeBloodPressureNoteAsync(BloodPressure info);

        /// <summary>
        ///     Find heartbeat by using heartbeat note id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<IList<BloodPressure>> FindBloodPressureNoteAsync(int id, int? owner);

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseBloodPressureFilter> FilterBloodPressureNoteAsync(FilterBloodPressureViewModel filter);


        /// <summary>
        ///     Delete a heartbeat note asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        Task<int> DeleteBloodPressureNoteAsync(int id, int owner);
    }
}