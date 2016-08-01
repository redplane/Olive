using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Shared.Models;

namespace Olives.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Initialize notification to database and broadcast it.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="signalrHub">Hub which is used for broadcasting notification</param>
        /// <returns></returns>
        Task<Notification> BroadcastNotificationAsync(Notification notification, IHubContext signalrHub);   
    }
}