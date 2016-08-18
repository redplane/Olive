using System.Threading.Tasks;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Models;

namespace Olives.Interfaces.Medical
{
    public interface IRepositoryExperimentNote
    {
        /// <summary>
        ///     Find an experiment note asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ExperimentNote> FindExperimentNoteAsync(int id);

        /// <summary>
        ///     Initialize experment note with information.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        Task<ExperimentNote> InitializeExperimentNoteAsync(ExperimentNote note);

        /// <summary>
        ///     Delete experiment record or its infos only.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteExperimentNoteAsync(FilterExperimentNoteViewModel filter);

        /// <summary>
        ///     Filter experiment note asynchronously by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseExperimentNoteFilter> FilterExperimentNoteAsync(FilterExperimentNoteViewModel filter);
    }
}