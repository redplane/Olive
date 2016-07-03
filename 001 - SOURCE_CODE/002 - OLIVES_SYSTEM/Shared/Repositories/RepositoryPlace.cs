using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryPlace : IRepositoryPlace
    {
        #region Country

        /// <summary>
        /// Find countries by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="nameComparision"></param>
        /// <returns></returns>
        public async Task<IList<Country>> FindCountryAsync(int? id, string name, StringComparison? nameComparision)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // This variable is to check whether condition has been used or not.
            var conditionDeclared = false;

            // Find the countries using id.
            IQueryable<Country> results = context.Countries;

            // Id has been specified.
            if (id != null)
            {
                results = results.Where(x => x.Id == id);
                conditionDeclared = true;
            }

            // Name has been specified.
            if (!string.IsNullOrWhiteSpace(name))
            {
                results = results.Where(x => x.Name.Equals(name, nameComparision ?? StringComparison.Ordinal));
                conditionDeclared = true;
            }

            // No condition has been specified.
            if (!conditionDeclared)
                return null;

            return await results.ToListAsync();
        }


        /// <summary>
        /// Initialize a country asynchronously.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Country> InitializeCountryAsync(Country country)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add/update data to table.
            context.Countries.AddOrUpdate(country);
            await context.SaveChangesAsync();

            return country;
        }

        /// <summary>
        /// Edit a country asynchronously.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Country> EditCountryAsync(Country country)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Begin a transaction.
            // We have to use transaction because when city's name is changed, all country name fields in city table should be changed.    
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Countries.AddOrUpdate(country);
                    await context.Cities.Where(x => x.CountryId == country.Id)
                        .ForEachAsync(x => x.CountryName = country.Name);

                    await context.SaveChangesAsync();
                    transaction.Commit();

                    return country;
                }
                catch
                {
                    // Exception occurs, rollback the transaction to prevent data from being changed.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Filter countries list asynchronously with given conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseCountryFilter> FilterCountryAsync(FilterCountryViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Take all result set.
            IQueryable<Country> results = context.Countries;

            #region Filter 

            // Name has been specified.
            if (!string.IsNullOrWhiteSpace(filter.Name))
                results = results.Where(x => x.Name.Contains(filter.Name));

            // Count the matched results.
            var total = await results.CountAsync();

            // Sort the result.
            if (filter.Direction == SortDirection.Ascending)
                results = results.OrderBy(x => x.Name);
            else
                results = results.OrderByDescending(x => x.Name);

            #endregion

            // Do pagination.
            var skippedRecords = filter.Page * filter.Records;

            var filterResponse = new ResponseCountryFilter();
            filterResponse.Countries = await results.Skip(skippedRecords)
                .Take(filter.Records)
                .ToListAsync();

            filterResponse.Total = total;

            return filterResponse;
        }
        #endregion

        #region City

        /// <summary>
        /// Find a list of cities by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<City>> FindCityAsync(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the countries using id.
            var results = context.Cities.Where(x => x.Id == id);

            return await results.ToListAsync();
        }

        /// <summary>
        /// Initialize a city to database.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<City> InitializeCityAsync(City city)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the countries using id.
            context.Cities.AddOrUpdate(city);
            await context.SaveChangesAsync();

            return city;
        }

        /// <summary>
        /// Filter a list of cities asynchronously with given information.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseCityFilter> FilterCityAsync(FilterCityViewModel filter)
        {
            // Database context initialize.
            var context = new OlivesHealthEntities();

            // By default, take all cities.
            IQueryable<City> cities = context.Cities;

            // By default, take all countries.
            IQueryable<Country> countries = context.Countries;

            #region Cities filter
            
            // City name is specified.
            if (!string.IsNullOrWhiteSpace(filter.Name))
                cities = cities.Where(x => x.Name.Contains(filter.Name));

            #endregion

            #region Countries filter

            // Country id has been specified.
            if (filter.CountryId != null)
            {
                cities = cities.Where(x => x.CountryId == filter.CountryId);
                countries = countries.Where(x => x.Id == filter.CountryId);
            }

            // Country name has been specified.
            if (!string.IsNullOrWhiteSpace(filter.CountryName))
                countries = countries.Where(x => x.Name.Contains(filter.CountryName));

            #endregion

            // Response initialization.
            var response = new ResponseCityFilter();

            // Join tables and retrieve all records.
            var results = from city in cities
                          join country in countries on city.CountryId equals country.Id
                          select new
                          {
                              Cities = city,
                              Countries = country
                          };
            
            // Calculate the total records.
            response.Total = await results.CountAsync();

            // Calculate the number of records should be skipped.
            var skippedRecords = filter.Page * filter.Records;

            #region Sort

            switch (filter.Sort)
            {
                case CityResultSort.CityId:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        results = results.OrderBy(x => x.Cities.Id);
                        break;
                    }
                    results = results.OrderByDescending(x => x.Cities.Id);
                    break;

                case CityResultSort.CountryId:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        results = results.OrderBy(x => x.Countries.Id);
                        break;
                    }
                    results = results.OrderByDescending(x => x.Countries.Id);
                    break;
                case CityResultSort.CountryName:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        results = results.OrderBy(x => x.Countries.Name);
                        break;
                    }
                    results = results.OrderByDescending(x => x.Countries.Name);
                    break;
                default:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        results = results.OrderBy(x => x.Cities.Name);
                        break;
                    }
                    results = results.OrderByDescending(x => x.Cities.Name);
                    break;
            }

            #endregion

            // Response records construction.
            response.Cities = await results.Skip(skippedRecords)
                .Take(filter.Records)
                .Select(x => new CityViewModel()
                {
                    Id = x.Cities.Id,
                    Name = x.Cities.Name,
                    Country = new CountryViewModel()
                    {
                        Id = x.Countries.Id,
                        Name = x.Countries.Name
                    }
                })
                .ToListAsync();

            return response;
        }

        #endregion
    }
}