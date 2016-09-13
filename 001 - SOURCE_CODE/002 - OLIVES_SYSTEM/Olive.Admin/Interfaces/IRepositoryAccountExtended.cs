using System.Collections.Generic;
using System.Threading.Tasks;
using OliveAdmin.ViewModels.Filter;
using OliveAdmin.ViewModels.Statistic;
using Shared.Interfaces;
using Shared.Models.Vertexes;
using Shared.ViewModels.Response;

namespace OliveAdmin.Interfaces
{
    public interface IRepositoryAccountExtended : IRepositoryAccount
    {
        /// <summary>
        /// Find an admin by using specific conditions synchronously.
        /// </summary>
        /// <param name="filterAdminViewModel"></param>
        /// <returns></returns>
        Admin FindAdmin(FilterAdminViewModel filterAdminViewModel);

        /// <summary>
        /// Find an admin by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filterAdminViewModel"></param>
        /// <returns></returns>
        Task<Admin> FindAdminAsync(FilterAdminViewModel filterAdminViewModel);
        
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
    }
}