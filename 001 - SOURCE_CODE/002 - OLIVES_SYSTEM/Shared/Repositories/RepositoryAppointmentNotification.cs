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
    public class RepositoryAppointmentNotification : IRepositoryAppointmentNotification
    {
        /// <summary>
        ///     Filter appointment notification with specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseAppointmentNotificationFilter> FilterAppointmentNotificationAsync(
            FilterAppointmentNotificationViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<AppointmentNotification> appointmentNotifications = context.AppointmentNotifications;

            // Id of appointment notification.
            if (filter.Id != null)
                appointmentNotifications = appointmentNotifications.Where(x => x.Id == filter.Id.Value);

            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                appointmentNotifications = appointmentNotifications.Where(x => x.Recipient == filter.Requester);
                if (filter.Partner != null)
                    appointmentNotifications = appointmentNotifications.Where(x => x.Invoker == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                appointmentNotifications = appointmentNotifications.Where(x => x.Invoker == filter.Requester);
                if (filter.Partner != null)
                    appointmentNotifications = appointmentNotifications.Where(x => x.Recipient == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    appointmentNotifications =
                        appointmentNotifications.Where(
                            x => x.Invoker == filter.Requester || x.Recipient == filter.Requester);
                else
                    appointmentNotifications =
                        appointmentNotifications.Where(
                            x =>
                                (x.Invoker == filter.Requester && x.Recipient == filter.Partner.Value) ||
                                (x.Invoker == filter.Partner.Value && x.Recipient == filter.Requester));
            }

            // Appointment is specified.
            if (filter.Appointment != null)
                appointmentNotifications =
                    appointmentNotifications.Where(x => x.AppointmentId == filter.Appointment.Value);

            // IsSeen is specified.
            if (filter.IsSeen != null)
                appointmentNotifications = appointmentNotifications.Where(x => x.IsSeen == filter.IsSeen.Value);

            // Create is specified.
            if (filter.MinCreated != null)
                appointmentNotifications = appointmentNotifications.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null)
                appointmentNotifications = appointmentNotifications.Where(x => x.Created <= filter.MaxCreated.Value);

            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case AppointmentNotificationFilterSort.Appointment:
                            appointmentNotifications = appointmentNotifications.OrderBy(x => x.AppointmentId);
                            break;
                        case AppointmentNotificationFilterSort.Created:
                            appointmentNotifications = appointmentNotifications.OrderBy(x => x.Created);
                            break;
                        case AppointmentNotificationFilterSort.Seen:
                            appointmentNotifications = appointmentNotifications.OrderBy(x => x.IsSeen);
                            break;
                        default:
                            appointmentNotifications = appointmentNotifications.OrderBy(x => x.Type);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case AppointmentNotificationFilterSort.Appointment:
                            appointmentNotifications = appointmentNotifications.OrderByDescending(x => x.AppointmentId);
                            break;
                        case AppointmentNotificationFilterSort.Created:
                            appointmentNotifications = appointmentNotifications.OrderByDescending(x => x.Created);
                            break;
                        case AppointmentNotificationFilterSort.Seen:
                            appointmentNotifications = appointmentNotifications.OrderByDescending(x => x.IsSeen);
                            break;
                        default:
                            appointmentNotifications = appointmentNotifications.OrderByDescending(x => x.Type);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseAppointmentNotificationFilter();

            // Count the total matched result.
            response.Total = await appointmentNotifications.CountAsync();

            // Pagination is defined.
            if (filter.Records != null)
            {
                appointmentNotifications = appointmentNotifications.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Take the records.
            response.AppointmentNotifications = await appointmentNotifications.ToListAsync();

            return response;
        }

        /// <summary>
        ///     Initialize / update an appointment notification asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<AppointmentNotification> InitializeAppointmentNotificationAsync(
            AppointmentNotification initializer)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Begin the transaction.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Add / update notification.
                    context.AppointmentNotifications.AddOrUpdate(initializer);

                    // Save changes and commit the transaction.
                    await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return initializer;
                }
                catch
                {
                    // As exception occurs, rollback the transaction.
                    transaction.Rollback();

                    // Continue throwing exception.
                    throw;
                }
            }
        }
    }
}