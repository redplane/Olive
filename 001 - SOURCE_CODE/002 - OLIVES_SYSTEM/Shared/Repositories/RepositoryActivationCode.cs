using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Constants;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repositories
{
    public class RepositoryActivationCode : IRepositoryActivationCode
    {
        /// <summary>
        /// Initialize an allergy with given information.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="created"></param>
        public async Task<ActivationCode> InitializeActivationCodeAsync(int owner, DateTime created)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize a random code
            string code;
            while (true)
            {
                // Find the unique code.
                var initializedCode = CodeGeneratorHelper.Instance.Generate(FieldLength.ActivationCodeLength);
                var isCreated = await context.ActivationCodes.AnyAsync(x => x.Code.Equals(initializedCode));
                if (!isCreated)
                {
                    code = initializedCode;
                    break;
                }
            }
            

            var activationCode = new ActivationCode();
            activationCode.Code = code;
            activationCode.Expired = DateTime.Now.AddHours(Values.ActivationCodeHourDuration);
            activationCode.Owner = owner;
            
            // Add allergy to database context.
            context.ActivationCodes.AddOrUpdate(activationCode);

            // Submit allergy.
            await context.SaveChangesAsync();

            return activationCode;
        }

        /// <summary>
        /// Find activation code by owner and code.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IList<ActivationCode>> FindActivationCodeAsync(int? owner, string code)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
            
            // No filter is specified.
            if (owner == null && string.IsNullOrWhiteSpace(code))
                return null;

            // By default, retrieve all results.
            IQueryable<ActivationCode> results = context.ActivationCodes;

            // Owner is specified.
            if (owner != null)
                results = results.Where(x => x.Owner == owner);

            // Code is specified.
            if (!string.IsNullOrWhiteSpace(code))
                results = results.Where(x => x.Code.Equals(code));
            
            return await results.ToListAsync();
        }

        /// <summary>
        /// Delete an activation code synchronously.
        /// </summary>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        public async void DeleteActivationCode(ActivationCode activationCode)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Remove the specific result.
            context.ActivationCodes.Remove(activationCode);

            // Save result asynchronously.
            await context.SaveChangesAsync();
        } 
    }
}