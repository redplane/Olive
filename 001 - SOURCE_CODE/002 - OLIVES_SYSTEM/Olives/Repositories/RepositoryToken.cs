using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Shared.Constants;
using Shared.Enumerations;
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
        ///     Find activation code by owner and code.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<AccountCode> FindAccountCodeAsync(int? owner, byte? type, string code)
        {
            // No filter is specified.
            if (owner == null && string.IsNullOrWhiteSpace(code))
                return null;

            var context = _dataContext.Context;

            // By default, retrieve all results.
            IQueryable<AccountCode> results = context.AccountCodes;

            // Owner is specified.
            if (owner != null)
                results = results.Where(x => x.Owner == owner.Value);

            // Type is specified.
            if (type != null)
                results = results.Where(x => x.Type == type.Value);

            // Code is specified.
            if (!string.IsNullOrWhiteSpace(code))
                results = results.Where(x => x.Code.Equals(code));

            // Count the total result.
            var records = await results.CountAsync();

            // Result is not unique.
            if (records != 1)
                return null;

            return await results.FirstOrDefaultAsync();
        }
        
        #region New code

        /// <summary>
        /// Filter and detach the account tokens.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DetachAccountToken(FilterAccountTokenViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            // Take all the records.
            IQueryable<AccountCode> accountTokens = context.AccountCodes;

            // Filter and detach.
            accountTokens = FilterAccountTokens(accountTokens, filter);
            context.AccountCodes.RemoveRange(accountTokens);

            // Retrieve the number of records which have been deleted.
            var records = await context.SaveChangesAsync();

            return records;
        }
        
        /// <summary>
        /// Initialize account token.
        /// </summary>
        /// <param name="accountToken"></param>
        /// <returns></returns>
        public async Task<AccountCode> InitializeToken(AccountCode accountToken)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            context.AccountCodes.AddOrUpdate(x => new
            {
                x.Owner,
                x.Type
            }, accountToken);

            await context.SaveChangesAsync();

            return accountToken;
        }

        /// <summary>
        /// Find the account token by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<AccountCode> FindAccountTokenAsync(FilterAccountTokenViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            // Take all account tokens.
            IQueryable<AccountCode> accountCodes = context.AccountCodes;
            accountCodes = FilterAccountTokens(accountCodes, filter);

            return accountCodes.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Filter account tokens by using specific conditions.
        /// </summary>
        /// <param name="accountTokens"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<AccountCode> FilterAccountTokens(IQueryable<AccountCode> accountTokens, FilterAccountTokenViewModel filter)
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

        #endregion
    }
}