using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
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
        Task<ExperimentNote> InitializeExperimentNote(ExperimentNote note);

        /// <summary>
        ///     Delete experiment record or its infos only.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeleteExperimentNotesAsync(int experimentId, int? owner);

        /// <summary>
        ///     Filter experiment note asynchronously by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseExperimentNoteFilter> FilterExperimentNotesAsync(FilterExperimentNoteViewModel filter);
    }
}