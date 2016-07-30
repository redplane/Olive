using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repositories
{
    public class RepositoryCode : IRepositoryActivationCode
    {
        /// <summary>
        ///     Initialize an allergy with given information.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="type"></param>
        /// <param name="created"></param>
        public async Task<AccountCode> InitializeAccountCodeAsync(int owner, TypeAccountCode type, DateTime created)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize a random code
            string code;
            while (true)
            {
                // Find the unique code.
                var initializedCode = CodeGeneratorHelper.Instance.Generate(FieldLength.ActivationCodeLength);
                var isCreated = await context.AccountCodes.AnyAsync(x => x.Code.Equals(initializedCode));
                if (!isCreated)
                {
                    code = initializedCode;
                    break;
                }
            }


            var accountCode = new AccountCode();
            accountCode.Code = code;
            accountCode.Expired = DateTime.UtcNow.AddHours(Values.ActivationCodeHourDuration);
            accountCode.Owner = owner;
            accountCode.Type = (byte) type;

            // Add allergy to database context.
            context.AccountCodes.AddOrUpdate(accountCode);

            // Submit allergy.
            await context.SaveChangesAsync();

            return accountCode;
        }

        /// <summary>
        ///     Find activation code by owner and code.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<AccountCode> FindAccountCodeAsync(int? owner, byte? type, string code)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // No filter is specified.
            if (owner == null && string.IsNullOrWhiteSpace(code))
                return null;


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

        /// <summary>
        ///     Delete an activation code synchronously.
        /// </summary>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        public async void DeleteActivationCode(AccountCode activationCode)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Remove the specific result.
            context.AccountCodes.Remove(activationCode);

            // Save result asynchronously.
            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a new password to a target account by searching token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="password"></param>
        public async Task<int> InitializeNewAccountPassword(AccountCode token, string password)
        {
            // Database initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Find and delete the account code.
                    context.AccountCodes.RemoveRange(
                        context.AccountCodes.Where(
                            x => x.Code.Equals(token.Code) && x.Type == (byte) TypeAccountCode.ForgotPassword));

                    // Find and change the account code.
                    var person = await context.People.FirstOrDefaultAsync(x => x.Id == token.Owner);
                    person.Password = password;
                    context.People.AddOrUpdate(person);

                    // Save modifies.
                    var records = await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return records;
                }
                catch (Exception)
                {
                    // Error happens, throw the error after rollback the transaction.
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}