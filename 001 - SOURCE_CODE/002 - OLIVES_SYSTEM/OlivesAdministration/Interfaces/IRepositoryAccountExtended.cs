using System.Collections.Generic;
using System.Threading.Tasks;
using OlivesAdministration.ViewModels.Filter;
using OlivesAdministration.ViewModels.Statistic;
using Shared.Interfaces;
using Shared.ViewModels.Response;

namespace OlivesAdministration.Interfaces
{
    public interface IRepositoryAccountExtended : IRepositoryAccount
    {
        /// <summary>
        ///     Filter patient asynchronously with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePatientFilter> FilterPatientsAsync(FilterPatientViewModel filter);

        /// <summary>
        ///     This function is for filtering doctors with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseDoctorFilter> FilterDoctorsAsync(FilterDoctorViewModel filter);

        /// <summary>
        ///     Summary person by using role.
        /// </summary>
        /// <returns></returns>
        Task<IList<StatusSummaryViewModel>> SummarizePersonRoleAsync(byte? role);
    }
}