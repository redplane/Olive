using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olives.WebJob.Interfaces
{
    public interface IRepositoryOlives
    {
        /// <summary>
        ///     Find the expired activation code and delete 'em
        /// </summary>
        /// <returns></returns>
        Task<int> RemoveAllExpiredActivationCode();

        /// <summary>
        ///     This function is for finding appointment whose date is expired.
        /// </summary>
        /// <returns></returns>
        Task<int> HandleExpiredAppointmentsAsync();

        /// <summary>
        ///     This function is for searching and cleaning enlisted junk files.
        /// </summary>
        /// <returns></returns>
        Task<int> CleanJunkFilesAsync(List<Exception> exceptions);
    }
}