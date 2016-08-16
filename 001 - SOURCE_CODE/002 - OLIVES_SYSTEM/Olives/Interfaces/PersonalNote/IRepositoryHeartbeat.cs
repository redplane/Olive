using System.Threading.Tasks;
using Olives.ViewModels.Filter.Personal;
using Olives.ViewModels.Response.Personal;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.PersonalNote
{
    public interface IRepositoryHeartbeat
    {
        /// <summary>
        ///     Initialize and save heart
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Heartbeat> InitializeHeartbeatNoteAsync(Heartbeat info);

        /// <summary>
        ///     Find heartbeat by using heartbeat note id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Heartbeat> FindHeartbeatAsync(int id);

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseHeartbeatFilter> FilterHeartbeatAsync(FilterHeatbeatViewModel filter);

        /// <summary>
        ///     Delete a heartbeat note asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        Task<int> DeleteHeartbeatNoteAsync(FilterHeatbeatViewModel filter);
    }
}