using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Models;

namespace Olives.Interfaces.PersonalNote
{
    public interface IRepositoryDiary
    {
        /// <summary>
        /// Find diary by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Diary> FindDiaryAsync(int id);

        /// <summary>
        /// Initialize / update diary asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<Diary> InitializeDiaryAsync(Diary initializer);

        /// <summary>
        /// Delete all diary using filters asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteDiaryAsync(FilterDiaryViewModel filter);
        
        /// <summary>
        /// Filter diaries list by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseDiaryFilter> FilterDiariesAsync(FilterDiaryViewModel filter);
    }
}