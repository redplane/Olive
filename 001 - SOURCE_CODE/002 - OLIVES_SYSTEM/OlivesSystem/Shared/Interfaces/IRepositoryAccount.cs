using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.Nodes;
using Shared.ViewModels;

namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        Task<IPerson> LoginAsync(LoginViewModel info);

        Task<bool> CreatePersonAsync(IPerson info);

        /// <summary>
        ///     Check whether person with specific information exists or not.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailCaseSensitive"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<IPerson> GetPersonExistAsync(string email, bool emailCaseSensitive, string password, byte? role);

        /// <summary>
        ///     Filter person by using specific conditions.
        /// </summary>
        /// <param name="filter">Specific conditions.</param>
        /// <returns></returns>
        Task<IEnumerable<Person>> FilterPersonAsync(FilterPersonViewModel filter);

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="filter">Specific conditions</param>
        /// <returns></returns>
        Task<IEnumerable<Doctor>> FilterDoctorAsync(FilterDoctorViewModel filter);

        Task<object> UpdatePersonAsync(string id, IPerson info);
    }
}