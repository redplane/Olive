using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace OlivesAdministration.Test.Repositories
{
    public class RepositoryPlace : IRepositoryPlace
    {
        #region Properties

        /// <summary>
        /// List of places.
        /// </summary>
        public IList<Place> Places { get; set; } 

        #endregion

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
            // By default, take all records.
            IEnumerable<Place> places = new List<Place>(Places);

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
            response.Total = places.Count();
            
            // Record is defined.
            if (filter.Records != null)
            {
                places = places.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Result taking.
            response.Places = places.ToList();

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
            IEnumerable<Place> places = new List<Place>(Places);
            
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
        /// Initialize/update place to database.
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public async Task<Place> InitializePlaceAsync(Place place)
        {
            // Database context initialiazation.
            IList<Place> places = new List<Place>(Places);

            // Initialize/update place to database.
            var result = places.FirstOrDefault(x => x.Id == place.Id);
            if (result == null)
                places.Add(place);
            else
            {
                var index = places.IndexOf(result);
                places[index] = place;
            }
            
            return place;
        }

        public Task<Place> ModifyPlaceAsync(int id, Place place)
        {
            throw new NotImplementedException();
        }
    }
}