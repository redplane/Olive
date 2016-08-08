using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.Medical
{
    public interface IRepositoryMedicalRecord
    {
        /// <summary>
        /// Delete the medical record asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteMedicalRecordAsync(int id);

        /// <summary>
        ///     Initialize / edit a medical record asynchronously.
        /// </summary>
        /// <param name="medicalRecord"></param>
        /// <returns></returns>
        Task<MedicalRecord> InitializeMedicalRecordAsync(MedicalRecord medicalRecord);

        /// <summary>
        ///     Find a medical record by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MedicalRecord> FindMedicalRecordAsync(int id);

        /// <summary>
        ///     Filter medical record by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalRecordFilter> FilterMedicalRecordAsync(FilterMedicalRecordViewModel filter);
    }
}