using System.Threading.Tasks;

namespace Olives.Job.Interfaces
{
    public interface IRepositoryOlives
    {
        /// <summary>
        ///     Find the expired activation code and delete 'em
        /// </summary>
        /// <returns></returns>
        Task<int> FindAndCleanAllExpiredAccountTokens();

        /// <summary>
        /// Find and clean expired accounts.
        /// </summary>
        /// <returns></returns>
        Task<int> FindAndCleanAllExpiredAccounts();

        /// <summary>
        ///     This function is for finding appointment whose date is expired.
        /// </summary>
        /// <returns></returns>
        Task<int> FindAndHandleExpiredAppointments();

        /// <summary>
        ///     This function is for searching and cleaning enlisted junk files.
        /// </summary>
        /// <returns></returns>
        Task<int> FindAndCleanJunkFile();
    }
}