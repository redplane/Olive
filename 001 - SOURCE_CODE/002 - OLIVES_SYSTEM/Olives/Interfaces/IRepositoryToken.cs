using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Shared.Models;

namespace Olives.Interfaces
{
    public interface IRepositoryToken
    {
        /// <summary>
        ///     Find activation code by owner and code.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<AccountCode> FindAccountCodeAsync(int? owner, byte? type, string code);
        
        #region New 
        /// <summary>
        /// Initialize a token and link to an account.
        /// </summary>
        /// <param name="accountToken"></param>
        /// <returns></returns>
        Task<AccountCode> InitializeToken(AccountCode accountToken);

        /// <summary>
        /// Find the first account token by using specified conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<AccountCode> FindAccountTokenAsync(FilterAccountTokenViewModel filter);

        /// <summary>
        /// Find and remove the conditions matched account tokens.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DetachAccountToken(FilterAccountTokenViewModel filter);
        
        #endregion
    }
}