using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryMedical
    {
        /// <summary>
        /// Initialize / edit a medical record asynchronously.
        /// </summary>
        /// <param name="medicalRecord"></param>
        /// <returns></returns>
        Task<MedicalRecord> InitializeMedicalRecordAsync(MedicalRecord medicalRecord);

        /// <summary>
        /// Find a medical record by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MedicalRecord> FindMedicalRecordAsync(int id);

        /// <summary>
        /// Filter medical record by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalRecordFilter> FilterMedicalRecordAsync(FilterMedicalRecordViewModel filter);

        /// <summary>
        /// Initialize / update medical image.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<MedicalImage> InitializeMedicalImageAsync(MedicalImage info);

        /// <summary>
        /// Filter medical image by using id and owner.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalImageFilter> FilterMedicalImageAsync(FilterMedicalImageViewModel filter);

        /// <summary>
        /// Delete medical record by using id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeleteMedicalImageAsync(int id, int? owner);

        #region Prescription

        /// <summary>
        /// Find the prescription by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Prescription> FindPrescriptionAsync(int id);

        /// <summary>
        /// Initialize or update an prescription.
        /// </summary>
        /// <param name="prescription"></param>
        /// <returns></returns>
        Task<Prescription> InitializePrescriptionAsync(Prescription prescription);

        /// <summary>
        /// Delete prescription by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeletePrescriptionAsync(int id, int? owner);

        /// <summary>
        /// Filter prescription asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePrescriptionFilterViewModel> FilterPrescriptionAsync(
            FilterPrescriptionViewModel filter);

        #endregion
    }
}