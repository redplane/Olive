using System.Threading.Tasks;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.Medical
{
    public interface IRepositoryPrescriptionImage
    {
        /// <summary>
        ///     Find the prescription image asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PrescriptionImage> FindPrescriptionImageAsync(int id);

        /// <summary>
        ///     Initialize a prescription with input paramters.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<PrescriptionImage> InitializePrescriptionImage(PrescriptionImage initializer);

        /// <summary>
        ///     Initialize a prescription with input paramters.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeletePrescriptionImageAsync(int id, int? owner);

        /// <summary>
        ///     Filter prescription image.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePrescriptionImageFilter> FilterPrescriptionImageAsync(
            FilterPrescriptionImageViewModel filter);
    }
}