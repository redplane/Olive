using Olives.Models;
using Shared.Models;

namespace Olives.Interfaces
{
    public interface IRepositoryStorage
    {
        /// <summary>
        ///     Initialize a storage with name-relative path.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="relativePath"></param>
        void InitializeStorage(string name, string relativePath);

        /// <summary>
        ///     Find storage by using name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        StorageModel FindStorage(string name);
    }
}