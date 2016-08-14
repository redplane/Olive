using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces.Medical;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Repositories.Medical
{
    public class RepositoryPrescriptionImage : IRepositoryPrescriptionImage
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryPrescriptionImage(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find the prescription image asynchronously by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PrescriptionImage> FindPrescriptionImageAsync(int id)
        {
            var context = _dataContext.Context;
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
            var context = _dataContext.Context;
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
            var context = _dataContext.Context;
            
            // By default, take all records.
            IQueryable<PrescriptionImage> prescriptionImages = context.PrescriptionImages;

            // Find the medical image by using id.
            prescriptionImages = prescriptionImages.Where(x => x.Id == id);

            // Owner is defined.
            if (owner != null)
                prescriptionImages = prescriptionImages.Where(x => x.Owner == owner.Value);

            await prescriptionImages.ForEachAsync(x =>
            {
                x.Available = false;
            });
            
            // Count the number of affected records.
            var records = await context.SaveChangesAsync();
            
            // Tell the caller function the number of affected records.
            return records;
             
        }

        /// <summary>
        ///     Filter prescription image.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePrescriptionImageFilter> FilterPrescriptionImageAsync(
            FilterPrescriptionImageViewModel filter)
        {
            var context = _dataContext.Context;

            // By default, take all result.
            IQueryable<PrescriptionImage> prescriptionImages = context.PrescriptionImages;
            prescriptionImages = FilterPrescriptionImages(prescriptionImages, filter, context);

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

        /// <summary>
        /// Filter prescription by using specific conditions.
        /// </summary>
        /// <param name="prescriptionImages"></param>
        /// <param name="filter"></param>
        /// <param name="olivesHealthEntities"></param>
        /// <returns></returns>
        private IQueryable<PrescriptionImage> FilterPrescriptionImages(IQueryable<PrescriptionImage> prescriptionImages,
            FilterPrescriptionImageViewModel filter, OlivesHealthEntities olivesHealthEntities)
        {
            // Filter by using requester role.
            prescriptionImages = FilterPrescriptionByRequesterRole(prescriptionImages, filter, olivesHealthEntities);

            // Prescription is defined.
            prescriptionImages = prescriptionImages.Where(x => x.PrescriptionId == filter.Prescription);
            
            // Only take the available images.
            prescriptionImages = prescriptionImages.Where(x => x.Available);

            if (filter.Id != null)
                prescriptionImages = prescriptionImages.Where(x => x.Id == filter.Id);

            // Created is specified.
            if (filter.MinCreated != null)
                prescriptionImages = prescriptionImages.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null)
                prescriptionImages = prescriptionImages.Where(x => x.Created <= filter.MaxCreated.Value);

            return prescriptionImages;
        }

        /// <summary>
        /// Filter prescription by using specific conditions.
        /// </summary>
        /// <param name="prescriptionImages"></param>
        /// <param name="filter"></param>
        /// <param name="olivesHealthEntities"></param>
        /// <returns></returns>
        private IQueryable<PrescriptionImage> FilterPrescriptionByRequesterRole(
            IQueryable<PrescriptionImage> prescriptionImages, FilterPrescriptionImageViewModel filter,
            OlivesHealthEntities olivesHealthEntities)
        {

            // Requester is not defined.
            if (filter.Requester == null)
                throw new Exception("Requester must be specified.");

            // Patient only can see his/her records.
            if (filter.Requester.Role == (byte)Role.Patient)
            {
                prescriptionImages = prescriptionImages.Where(x => x.Owner == filter.Requester.Id);
                if (filter.Partner != null)
                    prescriptionImages = prescriptionImages.Where(x => x.Creator == filter.Partner.Value);

                return prescriptionImages;
            }

            // Doctor can see every record whose owner has connection to him/her.
            IQueryable<Relation> relationships = olivesHealthEntities.Relations;
            relationships = relationships.Where(x => x.Target == filter.Requester.Id);

            // Partner is specified. This means to be a patient
            // Only patient can send request to doctor, that means he/she is the source of relationship.
            if (filter.Partner != null)
                relationships = relationships.Where(x => x.Source == filter.Partner.Value);

            var results = from r in relationships
                          from m in prescriptionImages
                          where r.Source == m.Owner || r.Source == m.Creator
                          select m;

            return results;
        } 
        #endregion
    }
}