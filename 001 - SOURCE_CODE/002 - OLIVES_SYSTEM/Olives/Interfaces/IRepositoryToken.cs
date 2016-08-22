using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Shared.Models;

namespace Olives.Interfaces
{
    public interface IRepositoryToken
    {
        #region New 
        /// <summary>
        /// Initialize a token and link to an account.
        /// </summary>
        /// <param name="accountToken"></param>
        /// <returns></returns>
        Task<AccountToken> InitializeAccountTokenAsync(AccountToken accountToken);

        /// <summary>
        /// Find the first account token by using specified conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<AccountToken> FindAccountTokenAsync(FilterAccountTokenViewModel filter);

        /// <summary>
        /// Find and remove the conditions matched account tokens.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteAccountTokenAsync(FilterAccountTokenViewModel filter);
        
        #endregion
    }
}