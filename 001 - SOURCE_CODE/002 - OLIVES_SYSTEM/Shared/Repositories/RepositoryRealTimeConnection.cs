using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repositories
{
    public class RepositoryRealTimeConnection : IRepositoryRealTimeConnection
    {
        /// <summary>
        /// Find the real time connection by using account index and connection index.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Task<IEnumerable<RealTimeConnection>> FindRealTimeConnectionInfoAsync(string email, string connectionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialize / update a real time connection information asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<RealTimeConnection> InitializeRealTimeConnectionInfoAsync(RealTimeConnection initializer)
        {
            // Database connection initialization.
            var context = new OlivesHealthEntities();

            // Add or update real time connection information.
            context.RealTimeConnections.AddOrUpdate(initializer);
            
            // Save changes asynchrnously.
            await context.SaveChangesAsync();

            return initializer;
        }

        /// <summary>
        /// Delete a real time connection information.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailComparison"></param>
        /// <param name="connectionIndex"></param>
        /// <param name="connectionIndexComparison"></param>
        /// <returns></returns>
        public async Task<int> DeleteRealTimeConnectionInfoAsync(string email, StringComparison? emailComparison, string connectionIndex, StringComparison? connectionIndexComparison)
        {
            // Database connection initialize.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<RealTimeConnection> realTimeConnections = context.RealTimeConnections;

            // Email is defined.
            if (!string.IsNullOrWhiteSpace(email))
                realTimeConnections =
                    realTimeConnections.Where(x => x.Email.Equals(email, emailComparison ?? StringComparison.Ordinal));

            // Connection index is defined.
            if (!string.IsNullOrWhiteSpace(connectionIndex))
                realTimeConnections =
                    realTimeConnections.Where(
                        x =>
                            x.ConnectionId.Equals(connectionIndex, connectionIndexComparison ?? StringComparison.Ordinal));

            // Remove all filtered result.
            context.RealTimeConnections.RemoveRange(realTimeConnections);

            // Save changes asynchronously.
            var records = await context.SaveChangesAsync();

            return records;
        }
    }
}