using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositorySpecialty
    {
        /// <summary>
        ///     Find specialty by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Specialty> FindSpecialtyAsync(int id);

        /// <summary>
        ///     Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseSpecialtyFilter> FilterSpecialtyAsync(FilterSpecialtyViewModel filter);
    }
}