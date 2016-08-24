using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        /// <summary>
        ///     Find person by using specific information synchronously.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email of person</param>
        /// <param name="password">Password of person</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Person FindPerson(int? id, string email, string password, byte? role, StatusAccount? status);

        /// <summary>
        ///     Find person by using specific information asynchronously.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email of person</param>
        /// <param name="password">Password of person</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Person> FindPersonAsync(int? id, string email, string password, byte? role, StatusAccount? status);

        /// <summary>
        ///     Initialize or update person information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Person> InitializePersonAsync(Person info);

        /// <summary>
        /// Find the md5 password from the original one.
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        string FindMd5Password(string originalPassword);
    }
}