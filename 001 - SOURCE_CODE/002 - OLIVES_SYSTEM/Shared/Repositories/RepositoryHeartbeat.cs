using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryHeartbeat : IRepositoryHeartbeat
    {
        //public async Task<ResponseAllergyFilter> FilterAllergy(AllergyGetViewModel filter)
        //{
        //    // Database context initialization.
        //    var context = new OlivesHealthEntities();

        //    // By default, take all records.
        //    IQueryable<Allergy> results = context.Allergies;

        //    #region Result filtering

        //    // Id has been specified.
        //    if (filter.Id != null)
        //        results = results.Where(x => x.Id == filter.Id);

        //    // Owner has been specified.
        //    if (filter.Owner != null)
        //        results = results.Where(x => x.Owner == filter.Owner);

        //    // Name has been specified.
        //    if (!string.IsNullOrEmpty(filter.Name))
        //        results = results.Where(x => x.Name.Contains(filter.Name));

        //    // Cause has been specified.
        //    if (!string.IsNullOrEmpty(filter.Cause))
        //        results = results.Where(x => x.Cause.Contains(filter.Cause));

        //    // Note has been specified.
        //    if (!string.IsNullOrEmpty(filter.Note))
        //        results = results.Where(x => x.Note.Contains(filter.Note));

        //    // Either Min/Max Created has been specified.
        //    if (filter.MinCreated != null)
        //        results = results.Where(x => x.Created >= filter.MinCreated);
        //    if (filter.MaxCreated != null)
        //        results = results.Where(x => x.Created <= filter.MaxCreated);

        //    // Either Min/Max LastModified has been specified.
        //    if (filter.MinLastModified != null)
        //        results = results.Where(x => x.LastModified != null && x.LastModified >= filter.MinLastModified);
        //    if (filter.MaxLastModified != null)
        //        results = results.Where(x => x.LastModified != null && x.LastModified >= filter.MaxLastModified);

        //    #endregion

        //    // Initialize response.
        //    var response = new ResponseAllergyFilter();

        //    // Count the matched records before result truncation.
        //    response.Total = await results.CountAsync();

        //    // Calculate how many record should be skipped.
        //    var skippedRecords = filter.Records * filter.Page;

        //    response.Allergies = await results.OrderBy(x => x.Name)
        //        .Skip(skippedRecords)
        //        .Take(filter.Records)
        //        .ToListAsync();

        //    return response;
        //}

        /// <summary>
        /// Initialize heartbeat note to database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Heartbeat> InitializeHeartbeatNoteAsync(Heartbeat info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add allergy to database context.
            context.Heartbeats.AddOrUpdate(info);

            // Submit allergy.
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        /// Find heartbeat note by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <param name="owner">Allergy owner</param>
        /// <returns></returns>
        public async Task<IList<Heartbeat>> FindHeartbeatAsync(int id, int? owner)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
            
            // Find heartbeat note by using id.
            var results = context.Heartbeats.Where(x => x.Id == id);

            // Owner has been specified.
            if (owner != null)
                results = results.Where(x => x.Owner == owner);

            return await results.ToListAsync();
        }

        /// <summary>
        /// Delete a heartbeat note asynchrounously.
        /// </summary>
        /// <param name="heartbeatNote"></param>
        /// <returns></returns>
        public async void DeleteHeartbeatNoteAsync(Heartbeat heartbeatNote)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
            context.Heartbeats.Remove(heartbeatNote);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseHeartbeatFilter> FilterHeartbeatAsync(FilterHeatbeatViewModel filter)
        {
            // Data context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all information.
            IQueryable<Heartbeat> results = context.Heartbeats;

            // Owner has been specified.
            if (filter.Owner != null)
                results = results.Where(x => x.Owner == filter.Owner);

            // Rate has been specified.
            if (filter.MinRate != null)
                results = results.Where(x => x.Rate >= filter.MinRate);
            if (filter.MaxRate != null)
                results = results.Where(x => x.Rate <= filter.MaxRate);

            // Created has been specified.
            if (filter.MinCreated != null)
                results = results.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                results = results.Where(x => x.Created <= filter.MaxCreated);

            // LastModified has been specified.
            if (filter.MinLastModified != null)
                results = results.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                results = results.Where(x => x.LastModified <= filter.MaxLastModified);

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                results = results.Where(x => x.Note.Contains(filter.Note));

            // Order by last modified.
            results = results.OrderByDescending(x => x.LastModified);

            // Initialize response and throw result back.
            var response = new ResponseHeartbeatFilter();
            response.Total = await results.CountAsync();

            // Calculate what records should be shown up.
            var skippedRecords = filter.Page*filter.Records;
            response.Heartbeats = await results.Skip(skippedRecords)
                .Take(filter.Records)
                .Select(x => new HeartbeatViewModel()
                {
                    Created = x.Created,
                    Id = x.Id,
                    LastModified = x.LastModified,
                    Note = x.Note,
                    Rate = x.Rate
                })
                .ToListAsync();

            // Return filtered result.
            return response;
        }
    }
}