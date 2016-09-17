using System.Linq;
using System.Threading.Tasks;
using Shared.Models.Vertexes;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        /// <summary>
        ///     Find the md5 password from the original one.
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        string FindMd5Password(string originalPassword);

        /// <summary>
        /// Initialize an account asynchronously.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<Account> InitializeAccountAsync(Account account);
        
        /// <summary>
        ///     Find the first admin account which matches with the specific conditions synchronously.
        /// </summary>
        /// <param name="filterAccountViewModel"></param>
        /// <returns></returns>
        Account FindAccount(FilterAccountViewModel filterAccountViewModel);

        /// <summary>
        ///     Find the first admin account which matches with the specific conditions asynchronously.
        /// </summary>
        /// <param name="filterAdminViewModel"></param>
        /// <returns></returns>
        Task<Account> FindAccountAsync(FilterAccountViewModel filterAdminViewModel);
        
        /// <summary>
        ///     Filter accounts by using specific conditions with pagination.
        /// </summary>
        /// <param name="filterAccountViewModel"></param>
        /// <returns></returns>
        Task<ResponseAccountFilter> FilterAccountsAsync(FilterAccountViewModel filterAccountViewModel);
    }
}