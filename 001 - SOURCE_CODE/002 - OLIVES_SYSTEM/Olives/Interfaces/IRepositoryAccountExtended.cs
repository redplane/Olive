using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Response;

namespace Olives.Interfaces
{
    public interface IRepositoryAccountExtended : IRepositoryAccount
    {
        /// <summary>
        ///     Filter patients by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePatientFilter> FilterPatientsAsync(FilterPatientViewModel filter);

        /// <summary>
        ///     Filter doctor asynchronously with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseAccountFilter> FilterDoctorsAsync(FilterDoctorViewModel filter);

        /// <summary>
        ///     Edit a person by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Person> EditPersonProfileAsync(int id, Person info);
    }
}