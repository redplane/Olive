using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositorySpecialty
    {
        /// <summary>
        /// Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseSpecialtyFilter> FilterSpecialty(SpecialtyGetViewModel filter);

        /// <summary>
        /// Find a specialty by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<Specialty>> FindSpecialty(int id);
    }
}