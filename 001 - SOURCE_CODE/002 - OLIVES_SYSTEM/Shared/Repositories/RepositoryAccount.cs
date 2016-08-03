using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryAccount : IRepositoryAccount
    {
        #region Patient
        /// <summary>
        ///     Find and activate patient's account and remove the activation code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool> InitializePatientActivation(string code)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var results = from p in context.People
                        join c in
                            context.AccountCodes.Where(
                                x => x.Code.Equals(code) && x.Type == (byte) TypeAccountCode.Activation) on p.Id equals
                            c.Owner
                        select new
                        {
                            Person = p,
                            Code = c
                        };

                    // Retrieve the total matched result.
                    var resultsCount = await results.CountAsync();

                    // No result has been returned.
                    if (resultsCount < 1)
                        throw new InstanceNotFoundException($"Couldn't find person whose code is : {code}");

                    // Not only one result has been returned.
                    if (resultsCount > 1)
                        throw new Exception($"There are too many people whose code is : {code}");

                    // Retrieve the first queried result.
                    var result = await results.FirstOrDefaultAsync();
                    if (result == null)
                        throw new InstanceNotFoundException($"Couldn't find person whose code is : {code}");

                    // Update the person status and remove the activation code.
                    var person = result.Person;
                    var activationCode = result.Code;
                    person.Status = (byte) StatusAccount.Active;
                    context.People.AddOrUpdate(person);
                    context.AccountCodes.Remove(activationCode);
                    await context.SaveChangesAsync();
                    // Commit the transaction.
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    // Something happens, roll the transaction back.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion
        
        #region Shared

        /// <summary>
        ///     Find person by using email, password and role.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email which is used for filtering.</param>
        /// <param name="password">Password of account.</param>
        /// <param name="role">As role is specified. Find account by role.</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Person FindPerson(int? id, string email, string password, byte? role, StatusAccount? status)
        {
            // Database context intialize.
            var context = new OlivesHealthEntities();

            // By default, take all people in database.
            IQueryable<Person> result = context.People;

            // Id is specified.
            if (id != null)
                result = result.Where(x => x.Id == id);

            // Email is specified.
            if (!string.IsNullOrEmpty(email))
                result = result.Where(x => x.Email.Equals(email));

            // Password is specified.
            if (!string.IsNullOrEmpty(password))
                result = result.Where(x => x.Password.Equals(password));

            // Role is specified.
            if (role != null)
                result = result.Where(x => x.Role == role);

            if (status != null)
                result = result.Where(x => x.Status == (byte) status.Value);

            return result.FirstOrDefault();
        }

        /// <summary>
        ///     Find person by using email, password and role.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email which is used for filtering.</param>
        /// <param name="password">Password of account.</param>
        /// <param name="role">As role is specified. Find account by role.</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Person> FindPersonAsync(int? id, string email, string password, byte? role,
            StatusAccount? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all people in database.
            IQueryable<Person> result = context.People;

            // Id is specified.
            if (id != null)
                result = result.Where(x => x.Id == id);

            // Email is specified.
            if (!string.IsNullOrEmpty(email))
                result = result.Where(x => x.Email.Equals(email));

            // Password is specified.
            if (!string.IsNullOrEmpty(password))
                result = result.Where(x => x.Password.Equals(password));

            // Role is specified.
            if (role != null)
                result = result.Where(x => x.Role == role);

            // Status has been specified.
            if (status != null)
            {
                var castedStatus = (byte) status;
                result = result.Where(x => x.Status == castedStatus);
            }

            return await result.FirstOrDefaultAsync();
        }
        
        /// <summary>
        ///     Initialize or update person information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Person> InitializePersonAsync(Person info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add or update information base on the primary key.
            context.People.AddOrUpdate(info);

            // Save change to database.
            await context.SaveChangesAsync();

            return info;
        }

        #endregion
    }
}