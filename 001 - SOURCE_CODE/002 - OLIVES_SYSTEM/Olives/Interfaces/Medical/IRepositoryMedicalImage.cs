using System.Threading.Tasks;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Models;
namespace Olives.Interfaces.Medical
{
    public interface IRepositoryMedicalImage
    {
        /// <summary>
        ///     Initialize / update medical image.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<MedicalImage> InitializeMedicalImageAsync(MedicalImage info);

        /// <summary>
        ///     Filter medical image by using id and owner.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMedicalImageFilter> FilterMedicalImageAsync(FilterMedicalImageViewModel filter);

        /// <summary>
        ///     Delete medical record by using id
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteMedicalImageAsync(FilterMedicalImageViewModel filter);
    }
}