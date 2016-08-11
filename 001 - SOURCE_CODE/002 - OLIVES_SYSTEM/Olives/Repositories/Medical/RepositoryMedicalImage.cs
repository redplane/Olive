﻿using System.Data.Entity;
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

            // Filter by medical record id.
            medicalImages = medicalImages.Where(x => x.MedicalRecordId == filter.MedicalRecord);

            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                medicalImages = medicalImages.Where(x => x.Owner == filter.Requester);
                if (filter.Partner != null)
                    medicalImages = medicalImages.Where(x => x.Creator == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                medicalImages = medicalImages.Where(x => x.Creator == filter.Requester);
                if (filter.Partner != null)
                    medicalImages = medicalImages.Where(x => x.Owner == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    medicalImages =
                        medicalImages.Where(x => x.Creator == filter.Requester || x.Owner == filter.Requester);
                else
                    medicalImages =
                        medicalImages.Where(
                            x =>
                                (x.Creator == filter.Requester && x.Owner == filter.Partner.Value) ||
                                (x.Creator == filter.Partner.Value && x.Owner == filter.Requester));
            }

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
                medicalImages = medicalImages.Skip(filter.Page*filter.Records.Value)
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
            var context = _dataContext.Context;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
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
                        // This step is to tell background worker to take care the file which should be deleted.
                        var junkFile = new JunkFile();
                        junkFile.FullPath = x.FullPath;
                        context.JunkFiles.Add(junkFile);
                    });

                    // Remove all medical image records
                    context.MedicalImages.RemoveRange(medicalImages);

                    // Count the number of affected records.
                    var records = await context.SaveChangesAsync();

                    // Confirm doing transaction.
                    transaction.Commit();

                    // Tell the calling function the number of affected records.
                    return records;
                }
                catch
                {
                    // Error happens, rollback the transaction first.
                    transaction.Rollback();

                    // Let the calling function handle the exception.
                    throw;
                }
            }
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

        
        #endregion
    }
}