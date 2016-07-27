using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        /// <param name="owner"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Task<IEnumerable<RealTimeConnection>> FindRealTimeConnectionInfoAsync(int? owner, string connectionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the real time connection indexes by using specific conditions.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="connectionIndex"></param>
        /// <param name="connectionIndexComparison"></param>
        /// <returns></returns>
        public async Task<IList<string>> FindRealTimeConnectionIndexesAsync(int? owner, string connectionIndex, StringComparison? connectionIndexComparison)
        {
            // Database connection initialize.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<RealTimeConnection> realTimeConnections = context.RealTimeConnections;

            // Email is defined.
            if (owner != null)
                realTimeConnections =
                    realTimeConnections.Where(x => x.Owner == owner.Value);

            // Connection index is defined.
            if (!string.IsNullOrWhiteSpace(connectionIndex))
                realTimeConnections =
                    realTimeConnections.Where(
                        x =>
                            x.ConnectionId.Equals(connectionIndex, connectionIndexComparison ?? StringComparison.Ordinal));

            var connectionIndexes = await realTimeConnections.Select(x => x.ConnectionId).ToListAsync();
            return connectionIndexes;
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
            context.RealTimeConnections.Add(initializer);

            // Save changes asynchrnously.
            await context.SaveChangesAsync();
            
            return initializer;
        }

        /// <summary>
        /// Delete a real time connection information.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="connectionIndex"></param>
        /// <param name="connectionIndexComparison"></param>
        /// <returns></returns>
        public async Task<int> DeleteRealTimeConnectionInfoAsync(int? owner, string connectionIndex, StringComparison? connectionIndexComparison)
        {
            // Database connection initialize.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<RealTimeConnection> realTimeConnections = context.RealTimeConnections;

            // Email is defined.
            if (owner != null)
                realTimeConnections =
                    realTimeConnections.Where(x => x.Owner == owner.Value);

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