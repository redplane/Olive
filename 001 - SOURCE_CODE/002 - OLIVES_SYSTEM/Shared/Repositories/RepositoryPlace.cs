using System;
using System.Collections.Generic;
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
        public Task<int> DeletePlaceAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePlaceFilter> FilterPlacesAsync(FilterPlaceViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
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
            
            // Result taking.
            response.Places = await places.Skip(filter.Page*filter.Records)
                .Take(filter.Records)
                .ToListAsync();

            return response;
        }

        /// <summary>
        /// Find the place by using specific conditions asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="city"></param>
        /// <param name="cityComparision"></param>
        /// <param name="country"></param>
        /// <param name="countryComparison"></param>
        /// <returns></returns>
        public async Task<Place> FindPlaceAsync(int? id, string city, StringComparison? cityComparision, string country, StringComparison? countryComparison)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all places.
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

        public Task<Place> InitializePlaceAsync(Place place)
        {
            throw new NotImplementedException();
        }

        public Task<Place> ModifyPlaceAsync(int id, Place place)
        {
            throw new NotImplementedException();
        }
    }
}