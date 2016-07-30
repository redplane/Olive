using System;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IRepositoryActivationCode
    {
        /// <summary>
        ///     Initialize an allergy with given information.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="type"></param>
        /// <param name="created"></param>
        Task<AccountCode> InitializeAccountCodeAsync(int owner, TypeAccountCode type, DateTime created);

        /// <summary>
        ///     Find activation code by owner and code.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<AccountCode> FindAccountCodeAsync(int? owner, byte? type, string code);

        /// <summary>
        ///     Delete an activation code asynchronously.
        /// </summary>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        void DeleteActivationCode(AccountCode activationCode);

        /// <summary>
        ///     Initialize new password from forgot password token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<int> InitializeNewAccountPassword(AccountCode token, string password);
    }
}