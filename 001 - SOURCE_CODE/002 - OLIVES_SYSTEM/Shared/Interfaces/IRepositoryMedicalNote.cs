using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryMedicalNote
    {
        /// <summary>
        ///     Find the medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MedicalNote> FindMedicalNoteAsync(int id);

        /// <summary>
        ///     Initialize medical note by using specific information.
        /// </summary>
        /// <param name="medicalNote"></param>
        /// <returns></returns>
        Task<MedicalNote> InitializeMedicalNoteAsync(MedicalNote medicalNote);

        /// <summary>
        ///     Filter medical note by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalNoteFilter> FilterMedicalNotesAsync(FilterMedicalNoteViewModel filter);
    }
}