using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryAppointment : IRepositoryAppointment
    {
        /// <summary>
        ///     Initialize an appointment and save to database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Appointment> InitializeAppointment(Appointment info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Begin a transaction.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Add the appointment to database.
                    context.Appointments.AddOrUpdate(info);

                    // Save changes asynchronously.
                    await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();
                }
                catch
                {
                    // As exception is thrown, rollback the transaction first.
                    transaction.Rollback();

                    // Continue throwing exception.
                    throw;
                }
            }

            return info;
        }

        /// <summary>
        ///     Filter appointment asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseAppointmentFilter> FilterAppointmentAsync(FilterAppointmentViewModel filter)
        {
            // Data context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<Appointment> results = context.Appointments;

            // Id of appointment is defined.
            if (filter.Id != null)
                results = results.Where(x => x.Id == filter.Id.Value);

            // Filter by partner.
            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == PartnerFilterMode.PartnerIsMaker)
            {
                results = results.Where(x => x.Dater == filter.Requester);
                if (filter.Partner != null)
                    results = results.Where(x => x.Maker == filter.Partner);
            }
            else if (filter.Mode == PartnerFilterMode.ParterIsDater)
            {
                results = results.Where(x => x.Maker == filter.Requester);
                if (filter.Partner != null)
                    results = results.Where(x => x.Dater == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    results =
                        results.Where(x => x.Maker == filter.Requester || x.Dater == filter.Requester);
                else
                {
                    results =
                        results.Where(
                            x =>
                                (x.Maker == filter.Requester && x.Dater == filter.Partner) ||
                                (x.Maker == filter.Partner && x.Dater == filter.Requester));
                }
            }

            // Created is specified.
            if (filter.MinCreated != null)
                results = results.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                results = results.Where(x => x.Created <= filter.MaxCreated);

            // From is specified.
            if (filter.MinFrom != null)
                results = results.Where(x => x.From >= filter.MinFrom);
            if (filter.MaxFrom != null)
                results = results.Where(x => x.From <= filter.MaxFrom);

            // To is specified.
            if (filter.MinTo != null)
                results = results.Where(x => x.To >= filter.MinTo);
            if (filter.MaxTo != null)
                results = results.Where(x => x.To <= filter.MaxTo);

            // Appointment status is specified.
            if (filter.Status != null)
            {
                var status = (byte)filter.Status;
                results = results.Where(x => x.Status == status);
            }

            // LastModified is specified.
            if (filter.MinLastModified != null)
                results = results.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                results = results.Where(x => x.LastModified <= filter.MaxLastModified);

            // Response initialization.
            var response = new ResponseAppointmentFilter();

            // Count the records first.
            response.Total = await results.CountAsync();

            // By default, sort by last modified decending.
            results = results.OrderByDescending(x => x.LastModified);

            // Record is defined.
            if (filter.Records != null)
            {
                results = results.Skip(filter.Page * filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            response.Appointments = await results.OrderBy(x => x.Status)
                .ToListAsync();

            return response;
        }

        /// <summary>
        ///     Find appointment by using appointment id and requester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Appointment> FindAppointmentAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all appointment.
            IQueryable<Appointment> appointments = context.Appointments;

            // Find appointment by querying id.
            appointments = appointments.Where(x => x.Id == id);

            return await appointments.FirstOrDefaultAsync();
        }
    }
}