using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Models;
using Shared.ViewModels;
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
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<Prescription> FindPrescriptionAsync(int id, int? owner = null);

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
        
        #region Experiment note

        /// <summary>
        /// Find an experiment note asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ExperimentNote> FindExperimentNoteAsync(int id);

        /// <summary>
        /// Initialize experment note with information.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        Task<ExperimentNote> InitializeExperimentNote(ExperimentNote note);
        
        /// <summary>
        /// Delete experiment record or its infos only.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        Task<int> DeleteExperimentNotesAsync(int experimentId);

        #endregion
    }
}