using System;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryMedical
    {
        #region Medical record

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

        #endregion

        #region Medical image

        /// <summary>
        ///     Initialize / update medical image.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<MedicalImage> InitializeMedicalImageAsync(MedicalImage info);

        /// <summary>
        ///     Filter medical image by using id and owner.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalImageFilter> FilterMedicalImageAsync(FilterMedicalImageViewModel filter);

        /// <summary>
        ///     Delete medical record by using id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeleteMedicalImageAsync(int id, int? owner);

        #endregion

        #region Prescription

        /// <summary>
        ///     Find the prescription by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<Prescription> FindPrescriptionAsync(int id, int? owner = null);

        /// <summary>
        ///     Initialize or update an prescription.
        /// </summary>
        /// <param name="prescription"></param>
        /// <returns></returns>
        Task<Prescription> InitializePrescriptionAsync(Prescription prescription);

        /// <summary>
        ///     Delete prescription by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeletePrescriptionAsync(int id, int? owner);

        /// <summary>
        ///     Filter prescription asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePrescriptionFilterViewModel> FilterPrescriptionAsync(
            FilterPrescriptionViewModel filter);

        #endregion

        #region Prescription image

        /// <summary>
        /// Find the prescription image asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PrescriptionImage> FindPrescriptionImageAsync(int id);

        /// <summary>
        /// Initialize a prescription with input paramters.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<PrescriptionImage> InitializePrescriptionImage(PrescriptionImage initializer);

        /// <summary>
        /// Initialize a prescription with input paramters.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeletePrescriptionImageAsync(int id, int? owner);

        /// <summary>
        /// Filter prescription image.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePrescriptionImageFilter> FilterPrescriptionImageAsync(
            FilterPrescriptionImageViewModel filter);
        #endregion

        #region Experiment note

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
        /// Filter experiment note asynchronously by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseExperimentNoteFilter> FilterExperimentNotesAsync(FilterExperimentNoteViewModel filter);

        #endregion

        #region Medical note

        /// <summary>
        /// Find the medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MedicalNote> FindMedicalNoteAsync(int id);

        /// <summary>
        /// Initialize medical note by using specific information.
        /// </summary>
        /// <param name="medicalNote"></param>
        /// <returns></returns>
        Task<MedicalNote> InitializeMedicalNoteAsync(MedicalNote medicalNote);

        /// <summary>
        /// Filter medical note by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalNoteFilter> FilterMedicalNotesAsync(FilterMedicalNoteViewModel filter);

        #endregion

        #region Medical category

        /// <summary>
        /// Find medical category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        Task<MedicalCategory> FindMedicalCategoryAsync(int? id, string name, StringComparison? comparison);

        /// <summary>
        /// Initialize medical category.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<MedicalCategory> InitializeMedicalCategoryAsync(MedicalCategory initializer);

        /// <summary>
        /// Filter medical categories asynchrously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalCategoryFilter> FilterMedicalCategoryAsync(FilterMedicalCategoryViewModel filter);

        #endregion
    }
}