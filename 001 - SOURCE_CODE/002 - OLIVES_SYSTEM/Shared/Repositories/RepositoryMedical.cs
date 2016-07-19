using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryMedical : IRepositoryMedical
    {
        #region Medical record

        /// <summary>
        ///     Initialize / edit a medical record asynchronously.
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
        ///     Find a medical record by using specific id.
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
        ///     Initialize / update medical image.
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

        #endregion

        #region Medical images

        /// <summary>
        ///     Find medical images by using id and owner
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
        ///     Delete a medical image asynchronously.
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
        ///     Filter medical records by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalRecordFilter> FilterMedicalRecordAsync(FilterMedicalRecordViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<MedicalRecord> medicalRecords = context.MedicalRecords;

            // Creator is specified.
            if (filter.Creator != null)
                medicalRecords = medicalRecords.Where(x => x.Creator == filter.Creator);

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
            if (filter.MinLastModified != null)
                medicalRecords = medicalRecords.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                medicalRecords = medicalRecords.Where(x => x.LastModified <= filter.MaxLastModified);

            // Result sorting
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case MedicalRecordFilterSort.Created:
                            medicalRecords = medicalRecords.OrderBy(x => x.Created);
                            break;
                        case MedicalRecordFilterSort.Time:
                            medicalRecords = medicalRecords.OrderBy(x => x.Time);
                            break;
                        default:
                            medicalRecords = medicalRecords.OrderBy(x => x.LastModified);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case MedicalRecordFilterSort.Created:
                            medicalRecords = medicalRecords.OrderByDescending(x => x.Created);
                            break;
                        case MedicalRecordFilterSort.Time:
                            medicalRecords = medicalRecords.OrderByDescending(x => x.Time);
                            break;
                        default:
                            medicalRecords = medicalRecords.OrderByDescending(x => x.LastModified);
                            break;
                    }
                    break;
            }

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
        ///     Find the prescription asynchronously.
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
        ///     Initialize or update an prescription.
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
        ///     Delete prescription by using id.
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
        ///     Filter prescription asynchronously.
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

            // Medical record is defined.
            if (filter.MedicalRecord != null)
                prescriptions = prescriptions.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            // Owner is defined.
            if (filter.Owner != null)
                prescriptions = prescriptions.Where(x => x.Owner == filter.Owner.Value);

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

        #region Prescription image

        /// <summary>
        /// Find the prescription image asynchronously by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PrescriptionImage> FindPrescriptionImageAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the prescription image by using id.
            var prescriptionImage = await context.PrescriptionImages.FirstOrDefaultAsync(x => x.Id == id);
            return prescriptionImage;
        }

        /// <summary>
        /// Initialize an image for prescription.
        /// </summary>
        /// <param name="prescriptionImage"></param>
        /// <returns></returns>
        public async Task<PrescriptionImage> InitializePrescriptionImage(PrescriptionImage prescriptionImage)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.PrescriptionImages.AddOrUpdate(prescriptionImage);
                    await context.SaveChangesAsync();
                    transaction.Commit();

                    return prescriptionImage;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete prescription image by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeletePrescriptionImageAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<PrescriptionImage> prescriptionImages = context.PrescriptionImages;

            // Find the medical image by using id.
            prescriptionImages = prescriptionImages.Where(x => x.Id == id);

            context.PrescriptionImages.RemoveRange(prescriptionImages);
            return await context.SaveChangesAsync();
        }

        /// <summary>
        /// Filter prescription image.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePrescriptionImageFilter> FilterPrescriptionImageAsync(
            FilterPrescriptionImageViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all result.
            IQueryable<PrescriptionImage> prescriptionImages = context.PrescriptionImages;

            // Base on the filter mode to decide requester is data creator or owner.
            if (filter.Mode == PrescriptionImageFilterMode.RequesterIsCreator)
            {
                prescriptionImages = prescriptionImages.Where(x => x.Creator == filter.Requester);
                if (filter.Partner != null)
                    prescriptionImages = prescriptionImages.Where(x => x.Owner == filter.Partner);
            }
            else if (filter.Mode == PrescriptionImageFilterMode.RequesterIsOwner)
            {
                prescriptionImages = prescriptionImages.Where(x => x.Owner == filter.Requester);
                if (filter.Partner != null)
                    prescriptionImages = prescriptionImages.Where(x => x.Creator == filter.Partner);
            }
            else
            {
                prescriptionImages =
                    prescriptionImages.Where(x => x.Creator == filter.Requester || x.Owner == filter.Requester);
            }
            
            // Filter response initialization.
            var response = new ResponsePrescriptionImageFilter();
            
            // Count the condition matched results.
            response.Total = await prescriptionImages.CountAsync();

            // Calculate the skipped result.
            var skippedResult = filter.Page*filter.Records;

            // Truncate the result.
            response.PrescriptionImages = await prescriptionImages.Skip(skippedResult)
                .Take(filter.Records)
                .ToListAsync();

            // By default, sort by created date decendingly.
            prescriptionImages = prescriptionImages.OrderByDescending(x => x.Created);

            return response;
        }

        #endregion

        #region Experiment

        /// <summary>
        ///     Find an experiment note asynchronously by using id.
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
        ///     Initialize experment note with information.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public async Task<ExperimentNote> InitializeExperimentNote(ExperimentNote note)
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
        ///     Delete experiment or its infos.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        public async Task<int> DeleteExperimentNotesAsync(int experimentId)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Retrieve all experiment infos.
                    context.ExperimentNotes.RemoveRange(context.ExperimentNotes.Where(x => x.Id == experimentId));
                    var deletedRecords = await context.SaveChangesAsync();
                    transaction.Commit();

                    return deletedRecords;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region Medical note

        /// <summary>
        /// Find the medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MedicalNote> FindMedicalNoteAsync(int id)
        {
            // Database context initialization
            var context = new OlivesHealthEntities();

            // Find the medical note by using id.
            return await context.MedicalNotes.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Initialize a medical note asynchronously.
        /// </summary>
        /// <param name="medicalNote"></param>
        /// <returns></returns>
        public async Task<MedicalNote> InitializeMedicalNoteAsync(MedicalNote medicalNote)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize/update medical notes.
            context.MedicalNotes.AddOrUpdate(medicalNote);

            // Save changes.
            await context.SaveChangesAsync();

            return medicalNote;
        }

        /// <summary>
        /// Filter medical notes asynchronously by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalNoteFilter> FilterMedicalNotesAsync(FilterMedicalNoteViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all record by searching creator id.
            IQueryable<MedicalNote> medicalNotes = context.MedicalNotes;
            medicalNotes = medicalNotes.Where(x => x.Creator == filter.Creator);

            // Medical record is defined.
            if (filter.MedicalRecord != null)
                medicalNotes = medicalNotes.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            // Owner is specified.
            if (filter.Owner != null)
                medicalNotes = medicalNotes.Where(x => x.Owner == filter.Owner);

            // Note is specified.
            if (filter.Note != null)
                medicalNotes = medicalNotes.Where(x => x.Note.Contains(filter.Note));

            // Time is specified.
            if (filter.MinTime != null) medicalNotes = medicalNotes.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null) medicalNotes = medicalNotes.Where(x => x.Time <= filter.MaxTime);

            // Created is defined.
            if (filter.MinCreated != null) medicalNotes = medicalNotes.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) medicalNotes = medicalNotes.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is defined.
            if (filter.MinLastModified != null)
                medicalNotes = medicalNotes.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                medicalNotes = medicalNotes.Where(x => x.LastModified <= filter.MaxLastModified);

            // Result sort.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case MedicalNoteFilterSort.Created:
                            medicalNotes = medicalNotes.OrderBy(x => x.Created);
                            break;
                        case MedicalNoteFilterSort.Note:
                            medicalNotes = medicalNotes.OrderBy(x => x.Note);
                            break;
                        case MedicalNoteFilterSort.Time:
                            medicalNotes = medicalNotes.OrderBy(x => x.Time);
                            break;
                        default:
                            medicalNotes = medicalNotes.OrderBy(x => x.LastModified);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case MedicalNoteFilterSort.Created:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.Created);
                            break;
                        case MedicalNoteFilterSort.Note:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.Note);
                            break;
                        case MedicalNoteFilterSort.Time:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.Time);
                            break;
                        default:
                            medicalNotes = medicalNotes.OrderByDescending(x => x.LastModified);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseMedicalNoteFilter();

            // Calculate the total matched records.
            response.Total = await medicalNotes.CountAsync();

            // Truncate the results.
            response.MedicalNotes = await medicalNotes.Skip(filter.Page * filter.Records)
                .Take(filter.Records)
                .ToListAsync();

            return response;
        }

        #endregion

        #region Medical category

        /// <summary>
        /// Find medical category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public async Task<MedicalCategory> FindMedicalCategoryAsync(int? id, string name, StringComparison? comparison)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
            
            // By default, take all record.
            IQueryable<MedicalCategory> medicalCategories = context.MedicalCategories;

            // Id is defined.
            if (id != null)
                medicalCategories = medicalCategories.Where(x => x.Id == id);

            // Name is defined.
            if (name != null)
                medicalCategories =
                    medicalCategories.Where(x => x.Name.Equals(name, comparison ?? StringComparison.Ordinal));

            return await medicalCategories.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Initialize medical category.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<MedicalCategory> InitializeMedicalCategoryAsync(MedicalCategory initializer)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize or update data.
            context.MedicalCategories.AddOrUpdate(initializer);

            // Save changes.
            await context.SaveChangesAsync();
            return initializer;
        }

        /// <summary>
        /// Filter medical categories asynchrously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalCategoryFilter> FilterMedicalCategoryAsync(
            FilterMedicalCategoryViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all categories.
            IQueryable<MedicalCategory> categories = context.MedicalCategories;

            // Name is defined.
            if (filter.Name != null)
                categories = categories.Where(x => x.Name.Contains(filter.Name));

            // Created is defined.
            if (filter.MinCreated != null) categories = categories.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) categories = categories.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is defined.
            if (filter.MinLastModified != null) categories = categories.Where(x => x.LastModified >= filter.MinCreated);
            if (filter.MaxLastModified != null)
                categories = categories.Where(x => x.LastModified >= filter.MaxLastModified);

            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case MedicalCategoryFilterSort.Created:
                            categories = categories.OrderBy(x => x.Created);
                            break;
                            case MedicalCategoryFilterSort.LastModified:
                            categories = categories.OrderBy(x => x.LastModified);
                            break;
                        default:
                            categories = categories.OrderBy(x => x.Name);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case MedicalCategoryFilterSort.Created:
                            categories = categories.OrderByDescending(x => x.Created);
                            break;
                        case MedicalCategoryFilterSort.LastModified:
                            categories = categories.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            categories = categories.OrderByDescending(x => x.Name);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseMedicalCategoryFilter();

            // Count the total matched result.
            response.Total = await categories.CountAsync();

            // Do pagination.
            response.MedicalCategories = await categories.Skip(filter.Page*filter.Records)
                .Take(filter.Records)
                .ToListAsync();

            return response;
        }

        #endregion
    }
}