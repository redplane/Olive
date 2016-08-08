using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.Medical
{
    public interface IRepositoryPrescription
    {
        /// <summary>
        ///     Find the prescription by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<Prescription> FindPrescriptionAsync(int id, int? owner = null);

        /// <summary>
        ///     Initialize or update an prescription.
        /// </summary>
        /// <param name="prescription"></param>
        /// <returns></returns>
        Task<Prescription> InitializePrescriptionAsync(Prescription prescription);

        /// <summary>
        ///     Delete prescription by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<int> DeletePrescriptionAsync(int id, int? owner);

        /// <summary>
        ///     Filter prescription asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePrescriptionFilterViewModel> FilterPrescriptionAsync(
            FilterPrescriptionViewModel filter);
    }
}