using System.Threading.Tasks;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Models;

namespace Olives.Interfaces.Medical
{
    public interface IRepositoryMedicalRecord
    {
        /// <summary>
        ///     Delete the medical record asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteMedicalRecordAsync(FilterMedicalRecordViewModel filter);

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