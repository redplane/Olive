using System;
using System.Linq;
using System.Threading.Tasks;
using Olives.WebJob.Interfaces;
using Shared.Models;

namespace Olives.WebJob.Repositories
{
    public class RepositoryActivationCode : IRepositoryActivationCode
    {
        /// <summary>
        /// Find all invalid activation code and delete 'em
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
                    // Find the invalid activation code and remove 'em.
                    context.AccountCodes.RemoveRange(context.AccountCodes.Where(x => x.Expired <= DateTime.Now));

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
    }
}