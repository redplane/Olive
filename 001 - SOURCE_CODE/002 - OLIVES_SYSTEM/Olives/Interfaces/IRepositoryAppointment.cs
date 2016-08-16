using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces
{
    public interface IRepositoryAppointment
    {
        /// <summary>
        ///     Initialize an appointment with specific information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Appointment> InitializeAppointment(Appointment info);

        /// <summary>
        ///     Filter appointment with requester account & password.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseAppointmentFilter> FilterAppointmentAsync(FilterAppointmentViewModel filter);

        /// <summary>
        ///     Find appointment by searching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Appointment> FindAppointmentAsync(int id);
    }
}