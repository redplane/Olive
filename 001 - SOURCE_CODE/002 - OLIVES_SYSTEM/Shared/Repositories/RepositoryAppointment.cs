using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
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

            context.Appointments.Add(info);
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        ///     Filter appointment asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="requester">The person who made or be dated</param>
        /// <returns></returns>
        public async Task<ResponseAppointmentFilter> FilterAppointmentAsync(FilterAppointmentViewModel filter,
            int requester)
        {
            // Data context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            var results = from p in context.People.Where(x => x.Id == requester)
                from a in context.Appointments
                where
                    p != null &&
                    ((filter.Mode == PartnerFilterMode.PartnerIsRequester && p.Id == a.Dater) ||
                     (filter.Mode == PartnerFilterMode.ParterIsDater && p.Id == a.Maker))
                select a;

            // Filter by partner.
            if (filter.Mode == PartnerFilterMode.PartnerIsRequester)
                results = results.Where(x => x.Maker == filter.Partner);
            else if (filter.Mode == PartnerFilterMode.ParterIsDater)
                results = results.Where(x => x.Dater == filter.Partner);

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
                var status = (byte) filter.Status;
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

            var skippedRecords = filter.Page*filter.Records;
            response.Appointments = await results.OrderBy(x => x.Status)
                .Skip(skippedRecords)
                .Take(filter.Records)
                .Select(x => new AppointmentViewModel
                {
                    Id = x.Id,
                    Created = x.Created,
                    Dater = new PersonViewModel
                    {
                        Id = x.Dater,
                        FirstName = x.DaterFirstName,
                        LastName = x.DaterLastName
                    },
                    Maker = new PersonViewModel
                    {
                        Id = x.Maker,
                        FirstName = x.MakerFirstName,
                        LastName = x.MakerLastName
                    },
                    From = x.From,
                    To = x.To,
                    LastModified = x.LastModified,
                    Note = x.Note,
                    Status = (StatusAccount) x.Status
                })
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