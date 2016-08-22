using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Repositories
{
    public class RepositoryToken : IRepositoryToken
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Properties

        public RepositoryToken(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion
        
        /// <summary>
        ///     Filter and detach the account tokens.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteAccountTokenAsync(FilterAccountTokenViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            // Take all the records.
            IQueryable<AccountToken> accountTokens = context.AccountTokens;

            // Filter and detach.
            accountTokens = FilterAccountTokens(accountTokens, filter);
            context.AccountTokens.RemoveRange(accountTokens);

            // Retrieve the number of records which have been deleted.
            var records = await context.SaveChangesAsync();

            return records;
        }

        /// <summary>
        ///     Initialize account token.
        /// </summary>
        /// <param name="accountToken"></param>
        /// <returns></returns>
        public async Task<AccountToken> InitializeAccountTokenAsync(AccountToken accountToken)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            context.AccountTokens.AddOrUpdate(x => new
            {
                x.Owner,
                x.Type
            }, accountToken);

            await context.SaveChangesAsync();

            return accountToken;
        }

        /// <summary>
        ///     Find the account token by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<AccountToken> FindAccountTokenAsync(FilterAccountTokenViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            // Take all account tokens.
            IQueryable<AccountToken> accountCodes = context.AccountTokens;
            accountCodes = FilterAccountTokens(accountCodes, filter);

            return accountCodes.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Filter account tokens by using specific conditions.
        /// </summary>
        /// <param name="accountTokens"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<AccountToken> FilterAccountTokens(IQueryable<AccountToken> accountTokens,
            FilterAccountTokenViewModel filter)
        {
            // Owner is specified.
            if (filter.Owner != null)
                accountTokens = accountTokens.Where(x => x.Owner == filter.Owner.Value);

            // Code is specified.
            if (!string.IsNullOrWhiteSpace(filter.Code))
                accountTokens = accountTokens.Where(x => x.Code.Contains(filter.Code));

            // Type is specified.
            if (filter.Type != null)
                accountTokens = accountTokens.Where(x => x.Type == filter.Type.Value);

            return accountTokens;
        }
    }
}