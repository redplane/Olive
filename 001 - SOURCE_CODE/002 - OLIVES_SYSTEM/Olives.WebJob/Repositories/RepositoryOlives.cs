using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Olives.WebJob.Interfaces;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Models;

namespace Olives.WebJob.Repositories
{
    public class RepositoryOlives : IRepositoryOlives
    {
        /// <summary>
        /// This function is for finding account which has invalid activation code and delete all of 'em.
        /// </summary>
        /// <returns>The number of deleted record.</returns>
        public async Task<int> RemoveAllExpiredActivationCode()
        {
            // Context initialization.
            var context = new OlivesHealthEntities();
            
            // Begin the transaction, remove all invalid activation code.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // By default, remove all pending account at the check point.
                    IQueryable<Person> expiredAccounts = context.People;
                    expiredAccounts = expiredAccounts.Where(x => x.Status == (byte) StatusAccount.Pending);
                    
                    // By default, find all the expired tokens.
                    IQueryable<AccountCode> expiredTokens = context.AccountCodes;
                    expiredTokens = expiredTokens.Where(x => x.Type == (byte) TypeAccountCode.Activation);
                    expiredTokens = expiredTokens.Where(x => x.Expired <= DateTime.UtcNow);
                    
                    // Join 2 tables : Person and AccountToken to find the invalid account.
                    var expireCollection = from p in expiredAccounts
                        join t in expiredTokens on p.Id equals t.Owner
                        select new
                        {
                            People = p,
                            Tokens = t
                        };

                    // Remove invalid accounts first.
                    context.People.RemoveRange(expireCollection.Select(x => x.People));
                    context.AccountCodes.RemoveRange(expireCollection.Select(x => x.Tokens));
                    
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
                    throw;
                }
            }
        }

        /// <summary>
        /// This function is for finding appointment whose date is expired.
        /// </summary>
        /// <returns></returns>
        public async Task<int> MakeAppointmentsExpired()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Get the current unix time.
            var unixTime = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

            // Find and update active appointment.
            IQueryable<Appointment> appointments = context.Appointments;
            appointments = appointments.Where(x => (x.Status == (byte) StatusAppointment.Active || x.Status == (byte)StatusAppointment.Pending));
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
        
    }
}