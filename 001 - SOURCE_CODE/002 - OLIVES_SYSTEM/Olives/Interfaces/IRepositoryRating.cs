using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces
{
    public interface IRepositoryRating
    {
        /// <summary>
        ///     Initialize or update rating to database..
        /// </summary>
        /// <param name="rating"></param>
        /// <param name="rated">Person who is rated.</param>
        /// <returns></returns>
        Task<Rating> InitializeRatingAsync(Rating rating, int rated);

        /// <summary>
        ///     Filter rates by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseRatingFilter> FilterRatingAsync(FilterRatingViewModel filter);
    }
}