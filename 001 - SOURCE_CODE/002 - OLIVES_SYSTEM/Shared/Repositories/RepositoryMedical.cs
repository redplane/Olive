using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryMedical : IRepositoryMedical
    {
        /// <summary>
        /// Initialize / edit a medical record asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<MedicalRecord> InitializeMedicalRecordAsync(MedicalRecord info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add or update the record.
            context.MedicalRecords.AddOrUpdate(info);

            // Save the record asynchronously.
            await context.SaveChangesAsync();
            return info;
        }

        /// <summary>
        /// Find a medical record by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MedicalRecord> FindMedicalRecordAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all record.
            IQueryable<MedicalRecord> results = context.MedicalRecords;

            // Find the record by using id.
            return await results.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Initialize / update medical image.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<MedicalImage> InitializeMedicalImageAsync(MedicalImage info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            context.MedicalImages.AddOrUpdate(info);
            await context.SaveChangesAsync();
            return info;
        }

        #region Medical images

        /// <summary>
        /// Find medical images by using id and owner
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalImageFilter> FilterMedicalImageAsync(FilterMedicalImageViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<MedicalImage> medicalImages = context.MedicalImages;

            // Filter by medical record id.
            medicalImages = medicalImages.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    medicalImages = medicalImages.OrderBy(x => x.Created);
                    break;
                default:
                    medicalImages = medicalImages.OrderByDescending(x => x.Created);
                    break;
            }

            // Response initialization.
            var response = new ResponseMedicalImageFilter();

            // Count the number of records matched with the conditions.
            response.Total = await medicalImages.CountAsync();

            // Calculate how many record should be skipped.
            var skippedRecords = filter.Records * filter.Page;
            medicalImages = medicalImages.Skip(skippedRecords)
                .Take(filter.Records);

            response.MedicalImages = await medicalImages.ToListAsync();
            return response;
        }

        /// <summary>
        /// Delete a medical image asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<int> DeleteMedicalImageAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<MedicalImage> medicalImages = context.MedicalImages;

            // Find the medical image by using id.
            medicalImages = medicalImages.Where(x => x.Id == id);

            // Owner is specified.
            if (owner != null)
                medicalImages = medicalImages.Where(x => x.Owner == owner);

            context.MedicalImages.RemoveRange(medicalImages);
            return await context.SaveChangesAsync();
        }

        /// <summary>
        /// Filter medical records by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalRecordFilter> FilterMedicalRecordAsync(FilterMedicalRecordViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<MedicalRecord> medicalRecords = context.MedicalRecords;

            // Owner is specified.
            if (filter.Owner != null)
                medicalRecords = medicalRecords.Where(x => x.Owner == filter.Owner);

            // Time is specified.
            if (filter.MinTime != null) medicalRecords = medicalRecords.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null) medicalRecords = medicalRecords.Where(x => x.Time <= filter.MaxTime);

            // Created is specified.
            if (filter.MinCreated != null) medicalRecords = medicalRecords.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) medicalRecords = medicalRecords.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is specified.
            if (filter.MinLastModified != null) medicalRecords = medicalRecords.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null) medicalRecords = medicalRecords.Where(x => x.LastModified <= filter.MaxLastModified);

            // Calculate the total result.
            var response = new ResponseMedicalRecordFilter();
            response.Total = await medicalRecords.CountAsync();

            // Calculate the number of records should be skipped.
            var skippedRecords = filter.Page * filter.Records;
            medicalRecords = medicalRecords.Skip(skippedRecords)
                .Take(filter.Records);

            response.MedicalRecords = await medicalRecords.ToListAsync();

            return response;
        }

        #endregion

        #region Prescription

        /// <summary>
        /// Find the prescription asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<Prescription> FindPrescriptionAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Take all prescriptions.
            IQueryable<Prescription> prescriptions = context.Prescriptions;

            // Find the prescription.
            prescriptions = prescriptions.Where(x => x.Id == id);

            // Owner is defined.
            if (owner != null)
                prescriptions = prescriptions.Where(x => x.Owner == owner.Value);

            return await prescriptions.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Initialize or update an prescription.
        /// </summary>
        /// <param name="prescription"></param>
        /// <returns></returns>
        public async Task<Prescription> InitializePrescriptionAsync(Prescription prescription)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize or update prescription.
            context.Prescriptions.AddOrUpdate(prescription);

            // Save change to database.
            await context.SaveChangesAsync();

            return prescription;
        }

        /// <summary>
        /// Delete prescription by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<int> DeletePrescriptionAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, delete all record.
            IQueryable<Prescription> prescriptions = context.Prescriptions;
            prescriptions = prescriptions.Where(x => x.Id == id);

            // Owner is specified.
            if (owner != null)
                prescriptions = prescriptions.Where(x => x.Owner == owner.Value);

            context.Prescriptions.RemoveRange(prescriptions);
            return await context.SaveChangesAsync();
        }

        /// <summary>
        /// Filter prescription asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePrescriptionFilterViewModel> FilterPrescriptionAsync(
            FilterPrescriptionViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all prescriptions.
            IQueryable<Prescription> prescriptions = context.Prescriptions;

            // Firstly, only take prescription of a target medical record.
            prescriptions = prescriptions.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            // From is defined.
            if (filter.MinFrom != null) prescriptions = prescriptions.Where(x => x.From >= filter.MinFrom);
            if (filter.MaxFrom != null) prescriptions = prescriptions.Where(x => x.From <= filter.MaxFrom);

            // To is defined.
            if (filter.MinTo != null) prescriptions = prescriptions.Where(x => x.To >= filter.MinTo);
            if (filter.MaxTo != null) prescriptions = prescriptions.Where(x => x.To <= filter.MaxTo);

            // Created is defined.
            if (filter.MinCreated != null) prescriptions = prescriptions.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) prescriptions = prescriptions.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is defined.
            if (filter.MinLastModified != null)
                prescriptions = prescriptions.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                prescriptions = prescriptions.Where(x => x.LastModified <= filter.MaxLastModified);

            // Sort the record.
            switch (filter.Sort)
            {
                case NoteResultSort.Created:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        prescriptions = prescriptions.OrderBy(x => x.Created);
                        break;
                    }
                    prescriptions = prescriptions.OrderByDescending(x => x.Created);
                    break;
                default:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        prescriptions = prescriptions.OrderBy(x => x.LastModified);
                        break;
                    }
                    prescriptions = prescriptions.OrderByDescending(x => x.LastModified);
                    break;
            }

            // Calculate the number of records should be skipped.
            var skippedRecord = filter.Page * filter.Records;

            // Response initialization.
            var response = new ResponsePrescriptionFilterViewModel();
            response.Total = await prescriptions.CountAsync();

            // Retrieve the list of results.
            response.Prescriptions = await prescriptions.Skip(skippedRecord)
                .Take(filter.Records)
                .ToListAsync();

            return response;
        }

        #endregion

        #region Prescripted medicine

        /// <summary>
        /// Initialize a prescripted medicine to database.
        /// </summary>
        /// <param name="prescriptedMedicine"></param>
        /// <returns></returns>
        public async Task<PrescriptedMedicine> InitializePrescriptedMedicineAsync(
            PrescriptedMedicine prescriptedMedicine)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize prescripted medicine to database.
            context.PrescriptedMedicines.Add(prescriptedMedicine);

            // Save the prescripted medicine to database.
            await context.SaveChangesAsync();

            return prescriptedMedicine;
        }

        /// <summary>
        /// Filter medicine by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePrescriptedMedicineFilterViewModel> FilterPrescriptedMedicineAsync(FilterPrescriptedMedicineViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Take the list of medicines.
            IQueryable<PrescriptedMedicine> medicines = context.PrescriptedMedicines;

            // Prescription is defined.
            if (filter.Prescription != null)
                medicines = medicines.Where(x => x.PrescriptionId == filter.Prescription);

            // Owner is specified.
            if (filter.Owner != null)
                medicines = medicines.Where(x => x.Owner == filter.Owner);

            // Medicine name is specified.
            if (!string.IsNullOrWhiteSpace(filter.Medicine))
                medicines = medicines.Where(x => x.MedicineName.Contains(filter.Medicine));

            // Quantity is defined.
            if (filter.MinQuantity != null)
                medicines = medicines.Where(x => x.Quantity >= filter.MinQuantity);
            if (filter.MaxQuantity != null)
                medicines = medicines.Where(x => x.Quantity <= filter.MaxQuantity);

            // Expired is defined.
            if (filter.MinExpired != null)
                medicines = medicines.Where(x => x.Expired >= filter.MinExpired);
            if (filter.MaxExpired != null)
                medicines = medicines.Where(x => x.Expired <= filter.MaxExpired);

            // Unit is defined.
            if (!string.IsNullOrWhiteSpace(filter.Unit))
                medicines = medicines.Where(x => x.Unit.Equals(filter.Medicine));

            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case PrescriptedMedicineSort.MedicineName:
                            medicines = medicines.OrderBy(x => x.MedicineName);
                            break;
                        case PrescriptedMedicineSort.Quantity:
                            medicines = medicines.OrderBy(x => x.Quantity);
                            break;
                        default:
                            medicines = medicines.OrderBy(x => x.Expired);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case PrescriptedMedicineSort.MedicineName:
                            medicines = medicines.OrderByDescending(x => x.MedicineName);
                            break;
                        case PrescriptedMedicineSort.Quantity:
                            medicines = medicines.OrderByDescending(x => x.Quantity);
                            break;
                        default:
                            medicines = medicines.OrderByDescending(x => x.Expired);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponsePrescriptedMedicineFilterViewModel();

            // Calculate how many records should be skipped.
            var skippedRecords = filter.Page * filter.Records;

            // Count the total matched result.
            response.Total = await medicines.CountAsync();
            response.PrescriptedMedicines = await medicines
                .Skip(skippedRecords)
                .Take(filter.Records)
                .ToListAsync();

            return response;
        }

        #endregion

        #region Experiment

        /// <summary>
        /// Find an experiment note asynchronously by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExperimentNote> FindExperimentNoteAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Take all record first.
            IQueryable<ExperimentNote> experiments = context.ExperimentNotes;
            return await experiments.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Initialize experment note with information.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<ExperimentNote> InitializeExperimentNote(ExperimentNote note, Dictionary<string, double> info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
            
            // Begin a transaction.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Initialize a note.
                    context.ExperimentNotes.AddOrUpdate(note);
                    await context.SaveChangesAsync();

                    if (info != null)
                    {
                        foreach (var key in info.Keys)
                        {
                            var experimentInfo = new ExperimentInfo();
                            experimentInfo.Key = key;
                            experimentInfo.Value = info[key];
                            experimentInfo.ExperimentId = note.Id;
                            
                            context.ExperimentInfoes.AddOrUpdate(experimentInfo);
                            await context.SaveChangesAsync();
                        }
                    }

                    // Commit the transaction.
                    transaction.Commit();

                    return note;
                }
                catch
                {
                    // Exception occurs, rollback the transaction and throw the exception.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Modify a list of experiment info.
        /// </summary>
        /// <param name="experimentNote"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        public async Task<IList<ExperimentInfoViewModel>> ModifyExperimentNotes(ExperimentNote experimentNote, IList<ExperimentInfoViewModel> infos)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // List of failed record.
            IList<ExperimentInfoViewModel> failedRecords = null;

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Update the experiment.
                    context.ExperimentNotes.AddOrUpdate(experimentNote);

                    foreach (var info in infos)
                    {
                        var result =
                            context.ExperimentInfoes.FirstOrDefault(x => x.ExperimentId == experimentNote.Id && x.Key.Equals(info.Key));

                        // No record has been found.
                        if (result == null)
                        {
                            if (failedRecords == null)
                                failedRecords = new List<ExperimentInfoViewModel>();

                            failedRecords.Add(new ExperimentInfoViewModel()
                            {
                                Key = info.Key,
                                Value = info.Value
                            });

                            transaction.Rollback();
                        }
                        else
                        {
                            // There is at least one record is failed.
                            if (failedRecords != null)
                                continue;

                            result.Value = info.Value;
                            context.ExperimentInfoes.AddOrUpdate(result);
                        }
                    }

                    // No failed record has been found.
                    if (failedRecords == null)
                    {
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }

                    return failedRecords;
                }
                catch (Exception)
                {
                    // Rollback the transaction.
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        public async void FilterExperimentAsync(FilterExperimentViewModel filter)
        {
            // Database context initialization.
        }

        /// <summary>
        /// Delete experiment or its infos.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="keys"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<ResponseNoteDelete> DeleteExperimentNotesAsync(int experimentId, HashSet<string> keys, NoteDeleteMode mode)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
            
            // Response initialization.
            var response = new ResponseNoteDelete();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    switch (mode)
                    {
                        case NoteDeleteMode.Note:

                            // Retrieve all experiment infos.
                            IQueryable<ExperimentInfo> experimentInfos = context.ExperimentInfoes;

                            // Find the experiment infos.
                            experimentInfos = experimentInfos.Where(x => x.ExperimentId == experimentId);
                            context.ExperimentInfoes.RemoveRange(experimentInfos);
                            response.Notes = await context.SaveChangesAsync();

                            // Retrieve all experiment note.
                            IQueryable<ExperimentNote> experimentNotes = context.ExperimentNotes;
                            experimentNotes = experimentNotes.Where(x => x.Id == experimentId);
                            context.ExperimentNotes.RemoveRange(experimentNotes);
                            response.Pairs = await context.SaveChangesAsync();
                            break;
                            
                        default:
                            foreach (var key in keys)
                            {
                                var result =
                                    context.ExperimentInfoes.FirstOrDefault(x => x.Key.Equals(key));

                                // No record has been found.
                                if (result == null)
                                    continue;

                                context.ExperimentInfoes.Remove(result);
                            }

                            response.Pairs = await context.SaveChangesAsync();
                            break;
                    }


                    // Commit the transaction.
                    transaction.Commit();
                    return response;
                }
                catch (Exception)
                {
                    // Rollback the transaction.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion
    }
}