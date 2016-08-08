using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryPlace : IRepositoryPlace
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructor

        public RepositoryPlace(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePlaceFilter> FilterPlacesAsync(FilterPlaceViewModel filter)
        {
            // By default, take all records.
            var context = _dataContext.Context;
            IQueryable<Place> places = context.Places;

            // City is defined.
            if (!string.IsNullOrWhiteSpace(filter.City))
                places = places.Where(x => x.City.Contains(filter.City));

            // Country is defined.
            if (!string.IsNullOrEmpty(filter.Country))
                places = places.Where(x => x.Country.Contains(filter.Country));

            // Response initialization.
            var response = new ResponsePlaceFilter();

            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case PlaceFilterSort.City:
                            places = places.OrderBy(x => x.City);
                            break;
                        case PlaceFilterSort.Country:
                            places = places.OrderBy(x => x.Country);
                            break;
                        default:
                            places = places.OrderBy(x => x.Id);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case PlaceFilterSort.City:
                            places = places.OrderByDescending(x => x.City);
                            break;
                        case PlaceFilterSort.Country:
                            places = places.OrderByDescending(x => x.Country);
                            break;
                        default:
                            places = places.OrderByDescending(x => x.Id);
                            break;
                    }
                    break;
            }

            // Count the total matched results.
            response.Total = await places.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                places = places.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Result taking.
            response.Places = await places
                .ToListAsync();

            return response;
        }

        /// <summary>
        ///     Find the place by using specific conditions asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="city"></param>
        /// <param name="cityComparision"></param>
        /// <param name="country"></param>
        /// <param name="countryComparison"></param>
        /// <returns></returns>
        public Place FindPlace(int? id, string city, StringComparison? cityComparision, string country,
            StringComparison? countryComparison)
        {
            // By default, take all places.
            var context = _dataContext.Context;
            IQueryable<Place> places = context.Places;

            // Id is specified.
            if (id != null)
                places = places.Where(x => x.Id == id);

            // City is defined.
            if (city != null)
                places = places.Where(x => x.City.Equals(city, cityComparision ?? StringComparison.Ordinal));

            // Country is defined.
            if (country != null)
                places = places.Where(x => x.Country.Equals(country, countryComparison ?? StringComparison.Ordinal));

            // Take the first result.
            return places.FirstOrDefault();
        }

        /// <summary>
        ///     Find the place by using specific conditions asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="city"></param>
        /// <param name="cityComparision"></param>
        /// <param name="country"></param>
        /// <param name="countryComparison"></param>
        /// <returns></returns>
        public async Task<Place> FindPlaceAsync(int? id, string city, StringComparison? cityComparision, string country,
            StringComparison? countryComparison)
        {
            // By default, take all places.
            var context = _dataContext.Context;
            IQueryable<Place> places = context.Places;

            // Id is specified.
            if (id != null)
                places = places.Where(x => x.Id == id);

            // City is defined.
            if (city != null)
                places = places.Where(x => x.City.Equals(city, cityComparision ?? StringComparison.Ordinal));

            // Country is defined.
            if (country != null)
                places = places.Where(x => x.Country.Equals(country, countryComparison ?? StringComparison.Ordinal));

            // Take the first result.
            return await places.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Initialize / update a place to database.
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public async Task<Place> InitializePlaceAsync(Place place)
        {
            var context = _dataContext.Context;

            // Initialize/update place to database.
            context.Places.AddOrUpdate(place);

            // Save changes asychronously.
            await context.SaveChangesAsync();

            return place;
        }

        #endregion
    }
}