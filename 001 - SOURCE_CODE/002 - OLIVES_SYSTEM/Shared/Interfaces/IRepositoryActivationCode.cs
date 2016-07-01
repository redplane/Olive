using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
namespace Shared.Interfaces
{
    public interface IRepositoryActivationCode
    {
        /// <summary>
        /// Initialize an allergy with given information.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="created"></param>
        Task<ActivationCode> InitializeActivationCodeAsync(int owner, DateTime created);

        /// <summary>
        /// Find activation code by owner and code.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<IList<ActivationCode>> FindActivationCodeAsync(int? owner, string code);

        /// <summary>
        /// Delete an activation code asynchronously.
        /// </summary>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        void DeleteActivationCode(ActivationCode activationCode);
    }
}