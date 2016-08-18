using System.Threading.Tasks;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Models;

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
        Task<PrescriptionImage> InitializePrescriptionImageAsync(PrescriptionImage initializer);

        /// <summary>
        ///     Initialize a prescription with input paramters.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeletePrescriptionImageAsync(FilterPrescriptionImageViewModel filter);

        /// <summary>
        ///     Filter prescription image.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePrescriptionImageFilter> FilterPrescriptionImageAsync(
            FilterPrescriptionImageViewModel filter);
    }
}