using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Shared.Models;
using Shared.ViewModels.Response;

namespace Olives.Interfaces
{
    public interface IRepositoryMessage
    {
        /// <summary>
        ///     Find a message by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Message> FindMessageAsync(int id);

        /// <summary>
        ///     Send a message asynchrnously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<Message> BroadcastMessageAsync(Message initializer);

        /// <summary>
        ///     Filter a messages list asynchronously with given conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseMessageFilter> FilterMessagesAsync(FilterMessageViewModel filter);
    }
}