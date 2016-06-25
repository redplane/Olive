using System.Threading.Tasks;
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
    }
}