using System;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryPlace
    {
        /// <summary>
        ///     Find place by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="city"></param>
        /// <param name="cityComparision"></param>
        /// <param name="country"></param>
        /// <param name="countryComparison"></param>
        /// <returns></returns>
        Task<Place> FindPlaceAsync(int? id, string city, StringComparison? cityComparision, string country,
            StringComparison? countryComparison);

        /// <summary>
        ///     Initialize a place asynchronously.
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        Task<Place> InitializePlaceAsync(Place place);

        /// <summary>
        ///     Modify a place by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        Task<Place> ModifyPlaceAsync(int id, Place place);

        /// <summary>
        ///     Delete place by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeletePlaceAsync(int id);

        /// <summary>
        ///     Filter places by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePlaceFilter> FilterPlacesAsync(FilterPlaceViewModel filter);
    }
}