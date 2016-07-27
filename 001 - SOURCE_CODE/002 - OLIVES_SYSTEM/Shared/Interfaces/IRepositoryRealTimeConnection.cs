﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IRepositoryRealTimeConnection
    {
        /// <summary>
        /// Initialize/update real time connection information.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<RealTimeConnection> InitializeRealTimeConnectionInfoAsync(RealTimeConnection initializer);

        /// <summary>
        /// Find real time connection information asynchronously.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Task<IEnumerable<RealTimeConnection>> FindRealTimeConnectionInfoAsync(int? owner, string connectionId);

        /// <summary>
        /// Find the real time connection indexes by using specific conditions.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="connectionIndex"></param>
        /// <param name="connectionIndexComparison"></param>
        /// <returns></returns>
        Task<IList<string>> FindRealTimeConnectionIndexesAsync(int? owner,
            string connectionIndex, StringComparison? connectionIndexComparison);

        /// <summary>
        /// Find and delete real time connection informations by using specific conditions.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="connectionIndex"></param>
        /// <param name="connectionIndexComparison"></param>
        /// <returns></returns>
        Task<int> DeleteRealTimeConnectionInfoAsync(int? owner,
            string connectionIndex, StringComparison? connectionIndexComparison);
    }
}