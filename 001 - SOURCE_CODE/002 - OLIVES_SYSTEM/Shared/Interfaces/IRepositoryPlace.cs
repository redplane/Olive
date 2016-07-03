using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryPlace
    {

        #region Country

        /// <summary>
        /// Find list of countries by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="nameComparision"></param>
        /// <returns></returns>
        Task<IList<Country>> FindCountryAsync(int? id, string name, StringComparison? nameComparision);

        /// <summary>
        /// Initialize a country asynchronously.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        Task<Country> InitializeCountryAsync(Country country);

        /// <summary>
        /// Edit a country asynchronously.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        Task<Country> EditCountryAsync(Country country);

        /// <summary>
        /// Filter countries list asynchronously with given conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseCountryFilter> FilterCountryAsync(FilterCountryViewModel filter);

        #endregion

        #region City

        /// <summary>
        /// Find list of cities by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<City>> FindCityAsync(int id);

        /// <summary>
        /// Initialize a city to database asynchronously.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        Task<City> InitializeCityAsync(City city);

        /// <summary>
        /// Filter a city asynchronously with given conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseCityFilter> FilterCityAsync(FilterCityViewModel filter);

        #endregion

    }
}