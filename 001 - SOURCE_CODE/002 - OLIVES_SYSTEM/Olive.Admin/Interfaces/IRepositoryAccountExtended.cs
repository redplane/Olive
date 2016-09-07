using System.Collections.Generic;
using System.Threading.Tasks;
using Olive.Admin.ViewModels.Filter;
using Olive.Admin.ViewModels.Filter.Response;
using Olive.Admin.ViewModels.Statistic;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Response;

namespace Olive.Admin.Interfaces
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
        Task<ResponseFilterDoctorViewModel> FilterDoctorsAsync(FilterDoctorViewModel filter);

        /// <summary>
        ///     Summary person by using role.
        /// </summary>
        /// <returns></returns>
        Task<IList<StatusSummaryViewModel>> SummarizePersonRoleAsync(byte? role);

        /// <summary>
        ///     Edit person profile asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Person> EditPersonProfileAsync(int id, Person info);
    }
}