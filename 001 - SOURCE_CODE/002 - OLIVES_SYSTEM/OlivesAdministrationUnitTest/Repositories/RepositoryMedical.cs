using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace OlivesAdministration.Test.Repositories
{
    class RepositoryMedical : IRepositoryMedical
    {
        public List<MedicalCategory> Categories { get; set; }

        public RepositoryMedical()
        {
            Categories = new List<MedicalCategory>();
        }
        public Task<int> DeleteExperimentNotesAsync(int experimentId)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteMedicalImageAsync(int id, int? owner)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeletePrescriptionAsync(int id, int? owner)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMedicalCategoryFilter> FilterMedicalCategoryAsync(FilterMedicalCategoryViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMedicalImageFilter> FilterMedicalImageAsync(FilterMedicalImageViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMedicalNoteFilter> FilterMedicalNotesAsync(FilterMedicalNoteViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMedicalRecordFilter> FilterMedicalRecordAsync(FilterMedicalRecordViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ResponsePrescriptionFilterViewModel> FilterPrescriptionAsync(FilterPrescriptionViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ExperimentNote> FindExperimentNoteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<MedicalCategory> FindMedicalCategoryAsync(int? id, string name, StringComparison? comparison)
        {
            IEnumerable<MedicalCategory> categoties = new List<MedicalCategory>(Categories);

            // Find categories by using Name
            categoties = categoties.Where(x => x.Name == name);


            return categoties.FirstOrDefault();
        }

        public Task<MedicalNote> FindMedicalNoteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MedicalRecord> FindMedicalRecordAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Prescription> FindPrescriptionAsync(int id, int? owner = default(int?))
        {
            throw new NotImplementedException();
        }

        public Task<ExperimentNote> InitializeExperimentNote(ExperimentNote note)
        {
            throw new NotImplementedException();
        }

        public async Task<MedicalCategory> InitializeMedicalCategoryAsync(MedicalCategory initializer)
        {
            Categories.Add(initializer);
            return initializer;
        }

        public Task<MedicalImage> InitializeMedicalImageAsync(MedicalImage info)
        {
            throw new NotImplementedException();
        }

        public Task<MedicalNote> InitializeMedicalNoteAsync(MedicalNote medicalNote)
        {
            throw new NotImplementedException();
        }

        public Task<MedicalRecord> InitializeMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            throw new NotImplementedException();
        }

        public Task<Prescription> InitializePrescriptionAsync(Prescription prescription)
        {
            throw new NotImplementedException();
        }
    }
}
