using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces.Medical;
using Shared.Enumerations.Filter;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Repositories.Medical
{
    public class RepositoryPrescriptionImage : IRepositoryPrescriptionImage
    {
        /// <summary>
        ///     Find the prescription image asynchronously by using id.
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
        ///     Initialize an image for prescription.
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
        ///     Delete prescription image by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<int> DeletePrescriptionImageAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // By default, take all records.
                    IQueryable<PrescriptionImage> prescriptionImages = context.PrescriptionImages;

                    // Find the medical image by using id.
                    prescriptionImages = prescriptionImages.Where(x => x.Id == id);

                    // Owner is defined.
                    if (owner != null)
                        prescriptionImages = prescriptionImages.Where(x => x.Owner == owner.Value);

                    // Go through every images we found, enlist each one to junk files list.
                    await prescriptionImages.ForEachAsync(x =>
                    {
                        // Enlist the file to junk file.
                        var junkFile = new JunkFile();
                        junkFile.FullPath = x.FullPath;
                        context.JunkFiles.Add(junkFile);
                    });

                    context.PrescriptionImages.RemoveRange(prescriptionImages);

                    // Count the number of affected records.
                    var records = await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    // Tell the caller function the number of affected records.
                    return records;
                }
                catch
                {
                    // Exception is thrown, rollback the transaction first.
                    transaction.Rollback();

                    // Throw the exception to the calling function.
                    throw;
                }
            }
        }

        /// <summary>
        ///     Filter prescription image.
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

            // Prescription is defined.
            prescriptionImages = prescriptionImages.Where(x => x.PrescriptionId == filter.Prescription);

            // Base on the filter mode to decide requester is data creator or owner.
            if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                prescriptionImages = prescriptionImages.Where(x => x.Creator == filter.Requester);
                if (filter.Partner != null)
                    prescriptionImages = prescriptionImages.Where(x => x.Owner == filter.Partner);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsOwner)
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

            // By default, sort by created date decendingly.
            prescriptionImages = prescriptionImages.OrderByDescending(x => x.Created);

            // Record is defined.
            if (filter.Records != null)
            {
                prescriptionImages = prescriptionImages.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Truncate the result.
            response.PrescriptionImages = await prescriptionImages
                .ToListAsync();

            return response;
        }
    }
}