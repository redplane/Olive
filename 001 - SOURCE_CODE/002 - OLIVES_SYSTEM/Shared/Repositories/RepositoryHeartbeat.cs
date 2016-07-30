﻿using System.Data.Entity;
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
    public class RepositoryHeartbeat : IRepositoryHeartbeat
    {
        /// <summary>
        ///     Initialize heartbeat note to database.
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
        ///     Find heartbeat note by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <returns></returns>
        public async Task<Heartbeat> FindHeartbeatAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find heartbeat note by using id.
            return await context.Heartbeats.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Delete a heartbeat note asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteHeartbeatNoteAsync(FilterHeatbeatViewModel filter)
        {
            try
            {
                // Database context initialization.
                var context = new OlivesHealthEntities();

                // By default, take all information.
                IQueryable<Heartbeat> heartbeats = context.Heartbeats;

                // Id is specified.
                if (filter.Id != null)
                    heartbeats = heartbeats.Where(x => x.Id == filter.Id.Value);

                // Owner has been specified.
                if (filter.Owner != null)
                    heartbeats = heartbeats.Where(x => x.Owner == filter.Owner);

                // Rate has been specified.
                if (filter.MinRate != null)
                    heartbeats = heartbeats.Where(x => x.Rate >= filter.MinRate);
                if (filter.MaxRate != null)
                    heartbeats = heartbeats.Where(x => x.Rate <= filter.MaxRate);

                // Created has been specified.
                if (filter.MinCreated != null)
                    heartbeats = heartbeats.Where(x => x.Created >= filter.MinCreated);
                if (filter.MaxCreated != null)
                    heartbeats = heartbeats.Where(x => x.Created <= filter.MaxCreated);

                // Time has been specified.
                if (filter.MinTime != null)
                    heartbeats = heartbeats.Where(x => x.Time >= filter.MinTime);
                if (filter.MaxTime != null)
                    heartbeats = heartbeats.Where(x => x.Time <= filter.MaxTime);

                // LastModified has been specified.
                if (filter.MinLastModified != null)
                    heartbeats = heartbeats.Where(x => x.LastModified >= filter.MinLastModified);
                if (filter.MaxLastModified != null)
                    heartbeats = heartbeats.Where(x => x.LastModified <= filter.MaxLastModified);

                // Note has been specified.
                if (!string.IsNullOrWhiteSpace(filter.Note))
                    heartbeats = heartbeats.Where(x => x.Note.Contains(filter.Note));

                context.Heartbeats.RemoveRange(heartbeats);
                var records = await context.SaveChangesAsync();
                return records;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseHeartbeatFilter> FilterHeartbeatAsync(FilterHeatbeatViewModel filter)
        {
            // Data context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all information.
            IQueryable<Heartbeat> heartbeats = context.Heartbeats;

            // Id is specified.
            if (filter.Id != null)
                heartbeats = heartbeats.Where(x => x.Id == filter.Id.Value);

            // Owner has been specified.
            if (filter.Owner != null)
                heartbeats = heartbeats.Where(x => x.Owner == filter.Owner);

            // Rate has been specified.
            if (filter.MinRate != null)
                heartbeats = heartbeats.Where(x => x.Rate >= filter.MinRate);
            if (filter.MaxRate != null)
                heartbeats = heartbeats.Where(x => x.Rate <= filter.MaxRate);

            // Created has been specified.
            if (filter.MinCreated != null)
                heartbeats = heartbeats.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                heartbeats = heartbeats.Where(x => x.Created <= filter.MaxCreated);

            // Time has been specified.
            if (filter.MinTime != null)
                heartbeats = heartbeats.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null)
                heartbeats = heartbeats.Where(x => x.Time <= filter.MaxTime);

            // LastModified has been specified.
            if (filter.MinLastModified != null)
                heartbeats = heartbeats.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                heartbeats = heartbeats.Where(x => x.LastModified <= filter.MaxLastModified);

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                heartbeats = heartbeats.Where(x => x.Note.Contains(filter.Note));

            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            heartbeats = heartbeats.OrderByDescending(x => x.Created);
                            break;
                        case NoteResultSort.LastModified:
                            heartbeats = heartbeats.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            heartbeats = heartbeats.OrderByDescending(x => x.Time);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            heartbeats = heartbeats.OrderBy(x => x.Created);
                            break;
                        case NoteResultSort.LastModified:
                            heartbeats = heartbeats.OrderBy(x => x.LastModified);
                            break;
                        default:
                            heartbeats = heartbeats.OrderBy(x => x.Time);
                            break;
                    }
                    break;
            }

            // Initialize response and throw result back.
            var response = new ResponseHeartbeatFilter();
            response.Total = await heartbeats.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                heartbeats = heartbeats.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Summarize results.
            response.Heartbeats = heartbeats;

            // Return filtered result.
            return response;
        }
    }
}