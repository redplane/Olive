using System.Threading.Tasks;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.Medical
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
        Task<ResponseMedicalNoteFilter> FilterMedicalNoteAsync(FilterMedicalNoteViewModel filter);

        /// <summary>
        ///     Delete medical notes by using filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteMedicalNoteAsync(FilterMedicalNoteViewModel filter);
    }
}