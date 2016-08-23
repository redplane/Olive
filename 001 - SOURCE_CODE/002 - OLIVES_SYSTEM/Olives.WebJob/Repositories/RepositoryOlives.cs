using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Olives.WebJob.Interfaces;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Services;

namespace Olives.WebJob.Repositories
{
    public class RepositoryOlives : IRepositoryOlives
    {
        #region Properties

        /// <summary>
        /// Instance for accessing time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        /// Millseconds of a day.
        /// </summary>
        private const double MillisecondOfDay = 8.64e+7;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a repository of Olives.
        /// </summary>
        public RepositoryOlives()
        {
            _timeService = new TimeService();
        }

        #endregion

        /// <summary>
        ///     Find expired activation codes and delete 'em all.
        /// </summary>
        /// <returns>The number of deleted record.</returns>
        public async Task<int> FindAndCleanAllExpiredAccountTokens()
        {
            // Database context initialization
            var context = new OlivesHealthEntities();

            // Retrieve the current UTC.
            var currentUtc = DateTime.UtcNow;

            // Begin the transaction, remove all invalid activation code.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Find all expired account tokens.
                    var expiredAccountTokens = context.AccountTokens.Where(x => x.Expired <= currentUtc);

                    // Delete all of 'em.
                    context.AccountTokens.RemoveRange(expiredAccountTokens);

                    // Save changed.
                    var records = await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return records;
                }
                catch
                {
                    // Rollback the transaction.
                    transaction.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        ///     Find expired  and delete 'em all.
        /// </summary>
        /// <returns>The number of deleted record.</returns>
        public async Task<int> FindAndCleanAllExpiredAccounts()
        {
            // Database context initialization
            var context = new OlivesHealthEntities();

            // Retrieve the current UTC.
            var unix = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Begin the transaction, remove all invalid activation code.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Find all expired account tokens.
                    var expiredAccounts = context.People.Where(x => x.Status == (byte)StatusAccount.Pending && x.Created + MillisecondOfDay < unix);
                    
                    // Loop through everybody.
                    await expiredAccounts.ForEachAsync(x =>
                    {
                        if (x.Role == (byte) Role.Doctor)
                            context.Doctors.RemoveRange(context.Doctors.Where(doctor => doctor.Id == x.Id));

                        if (x.Role == (byte) Role.Patient)
                            context.Patients.RemoveRange(context.Patients.Where(patient => patient.Id == x.Id));
                    });

                    // Delete all of 'em.
                    context.People.RemoveRange(expiredAccounts);

                    // Save changed.
                    var records = await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return records;
                }
                catch
                {
                    // Rollback the transaction.
                    transaction.Rollback();
                    return 0;
                }
            }
        }
        
        /// <summary>
        ///     This function is for finding appointment whose date is expired.
        /// </summary>
        /// <returns></returns>
        public async Task<int> FindAndHandleExpiredAppointments()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Get the current unix time.
            var unixTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Find and update active appointment.
            IQueryable<Appointment> appointments = context.Appointments;
            appointments =
                appointments.Where(
                    x => x.Status == (byte) StatusAppointment.Active || x.Status == (byte) StatusAppointment.Pending);
            appointments = appointments.Where(x => x.To <= unixTime);

            await appointments.ForEachAsync(x =>
            {
                // Change pending apointments to expired.
                if (x.Status == (byte) StatusAppointment.Pending)
                {
                    x.Status = (byte) StatusAppointment.Expired;
                    return;
                }

                // Change active appointments to done.
                x.Status = (byte) StatusAppointment.Done;
            });

            // Update appointments.
            var records = await context.SaveChangesAsync();
            return records;
        }

        /// <summary>
        ///     This function is for searching and cleaning enlisted junk files.
        /// </summary>
        /// <returns></returns>
        public async Task<int> FindAndCleanJunkFile()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            foreach (var junkFile in context.JunkFiles)
            {
                try
                {
                    // File is empty.
                    if (string.IsNullOrWhiteSpace(junkFile.FullPath))
                    {
                        context.JunkFiles.Remove(junkFile);
                        continue;
                    }

                    // File doesn't exist anymore.
                    if (!File.Exists(junkFile.FullPath))
                    {
                        context.JunkFiles.Remove(junkFile);
                        continue;
                    }

                    // Delete the file first.
                    File.Delete(junkFile.FullPath);

                    // Remove the junk file.
                    context.JunkFiles.Remove(junkFile);
                }
                catch
                {
                    // Let it be handled later.
                }
            }

            // Save the changes.
            var records = await context.SaveChangesAsync();

            return records;
        }
    }
}