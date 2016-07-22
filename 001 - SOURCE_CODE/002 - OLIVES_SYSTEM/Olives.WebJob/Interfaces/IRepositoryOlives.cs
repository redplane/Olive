using System.Threading.Tasks;

namespace Olives.WebJob.Interfaces
{
    public interface IRepositoryOlives
    {
        /// <summary>
        /// Find the expired activation code and delete 'em
        /// </summary>
        /// <returns></returns>
        Task<int> RemoveAllExpiredActivationCode();
    }
}