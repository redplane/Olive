using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IRepositoryTaskCheckAppointment
    {
        /// <summary>
        /// Initialize appointment checking task.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<TaskCheckAppointment> InitializeTaskCheckAppointment(TaskCheckAppointment initializer);

        /// <summary>
        /// Access to database and resume all appointment check function.
        /// </summary>
        /// <returns></returns>
        Task ResumeCheckAppointment();
    }
}