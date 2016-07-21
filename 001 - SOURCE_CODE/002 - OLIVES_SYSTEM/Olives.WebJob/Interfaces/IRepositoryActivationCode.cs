using System.Threading.Tasks;

namespace Olives.WebJob.Interfaces
{
    public interface IRepositoryActivationCode
    {
        /// <summary>
        /// Find the expired activation code and delete 'em
        /// </summary>
        /// <returns></returns>
        Task<int> RemoveAllExpiredActivationCode();
    }
}