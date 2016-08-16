using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces.Medical;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Repositories.Medical
{
    public class RepositoryMedicalImage : IRepositoryMedicalImage
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryMedicalImage(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find medical images by using id and owner
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMedicalImageFilter> FilterMedicalImageAsync(FilterMedicalImageViewModel filter)
        {
            // By default, take all records.
            var context = _dataContext.Context;
            IQueryable<MedicalImage> medicalImages = context.MedicalImages;
            medicalImages = FilterMedicalImages(medicalImages, filter, context);

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

            // Record is defined.
            if (filter.Records != null)
            {
                medicalImages = medicalImages.Skip(filter.Page * filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Calculate how many record should be skipped.
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
            var context = _dataContext.Context;

            // By default, take all records.
            IQueryable<MedicalImage> medicalImages = context.MedicalImages;

            // Find the medical image by using id.
            medicalImages = medicalImages.Where(x => x.Id == id);

            // Owner is specified.
            if (owner != null)
                medicalImages = medicalImages.Where(x => x.Owner == owner);

            // Go through every record and put the file path to must deleted list.
            await medicalImages.ForEachAsync(x =>
            {
                var junkFile = new JunkFile();
                junkFile.FullPath = x.FullPath;
                context.JunkFiles.Add(junkFile);

            });

            context.MedicalImages.RemoveRange(medicalImages);
            
            // Count the number of affected records.
            var records = await context.SaveChangesAsync();
            
            // Tell the calling function the number of affected records.
            return records;

        }

        /// <summary>
        ///     Initialize / update medical image.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<MedicalImage> InitializeMedicalImageAsync(MedicalImage info)
        {
            var context = _dataContext.Context;
            context.MedicalImages.AddOrUpdate(info);
            await context.SaveChangesAsync();
            return info;
        }

        /// <summary>
        /// Filter medical images by using specific conditions and based on requester role.
        /// </summary>
        /// <param name="medicalImages"></param>
        /// <param name="filter"></param>
        /// <param name="olivesHealthEntities"></param>
        /// <returns></returns>
        private IQueryable<MedicalImage> FilterMedicalImages(IQueryable<MedicalImage> medicalImages,
            FilterMedicalImageViewModel filter, OlivesHealthEntities olivesHealthEntities)
        {
            // Filter image by role.
            medicalImages = FilterMedicalImagesByRequesterRole(medicalImages, filter, olivesHealthEntities);

            // Filter by medical record id.
            medicalImages = medicalImages.Where(x => x.MedicalRecordId == filter.MedicalRecord);
            
            // Created is specified.
            if (filter.MinCreated != null)
                medicalImages = medicalImages.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null)
                medicalImages = medicalImages.Where(x => x.Created <= filter.MaxCreated.Value);

            return medicalImages;
        }

        /// <summary>
        /// Base on the requester role to do exact filter function.
        /// </summary>
        /// <param name="medicalImages"></param>
        /// <param name="filter"></param>
        /// <param name="olivesHealthEntities"></param>
        /// <returns></returns>
        private IQueryable<MedicalImage> FilterMedicalImagesByRequesterRole(IQueryable<MedicalImage> medicalImages,
            FilterMedicalImageViewModel filter, OlivesHealthEntities olivesHealthEntities)
        {
            // Requester is not defined.
            if (filter.Requester == null)
                throw new Exception("Requester must be specified.");

            // Patient only can see his/her records.
            if (filter.Requester.Role == (byte)Role.Patient)
            {
                medicalImages = medicalImages.Where(x => x.Owner == filter.Requester.Id);
                if (filter.Partner != null)
                    medicalImages = medicalImages.Where(x => x.Creator == filter.Partner.Value);

                return medicalImages;
            }

            // Doctor can see every record whose owner has connection to him/her.
            IQueryable<Relation> relationships = olivesHealthEntities.Relations;
            relationships = relationships.Where(x => x.Target == filter.Requester.Id);

            // Partner is specified. This means to be a patient
            // Only patient can send request to doctor, that means he/she is the source of relationship.
            if (filter.Partner != null)
                relationships = relationships.Where(x => x.Source == filter.Partner.Value);

            var results = from r in relationships
                          from m in medicalImages
                          where r.Source == m.Owner || r.Source == m.Creator
                          select m;

            return results;
        }

        #endregion
    }
}