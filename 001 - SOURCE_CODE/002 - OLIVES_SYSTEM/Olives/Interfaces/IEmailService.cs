using System.Threading.Tasks;

namespace Olives.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        ///     This function is for sending email contains activation code to client's email.
        /// </summary>
        /// <param name="destinations"></param>
        /// <param name="templateName"></param>
        /// <param name="data"></param>
        Task<bool> InitializeEmail(string[] destinations, string templateName, object data);
    }
}