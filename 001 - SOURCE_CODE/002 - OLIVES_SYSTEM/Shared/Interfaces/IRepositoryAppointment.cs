using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryAppointment
    {
        /// <summary>
        /// Check whether relation is available or not.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="relative"></param>
        /// <returns></returns>
        Task<bool> IsRelationAvailable(int owner, int relative);

        /// <summary>
        /// Initialize an appointment with specific information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Appointment> InitializeAppointment(Appointment info);

        /// <summary>
        /// Filter appointment with requester account & password.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ResponseAppointmentFilter> FilterAppointmentAsync(FilterAppointmentViewModel filter, string account,
            string password);
    }
}