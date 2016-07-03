using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Controllers
{
    [OlivesAuthorize(new[] { Role.Admin })]
    public class PlaceController : ApiParentController
    {
        #region Properties

        /// <summary>
        ///     Repository places DI
        /// </summary>
        private readonly IRepositoryPlace _repositoryPlace;

        /// <summary>
        /// Logger module DI , this is used for writing log.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize an instance of PlaceController.
        /// </summary>
        /// <param name="repositoryPlace"></param>
        /// <param name="log"></param>
        public PlaceController(IRepositoryPlace repositoryPlace, ILog log)
        {
            _repositoryPlace = repositoryPlace;
            _log = log;
        }

        #endregion

        #region Country

        /// <summary>
        /// Find a country by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/country")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCountry([FromUri]int id)
        {
            // Using id to find country.
            var countries = await _repositoryPlace.FindCountryAsync(id, null, null);

            // Records hasn't been found or not unique, treat the result as not found.
            if (countries == null || countries.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first result.
            var country = countries.FirstOrDefault();

            // Country is invalid.
            if (country == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Return the city information.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Country = new
                {
                    country.Id,
                    country.Name
                }
            });
        }

        /// <summary>
        /// Create a country.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/country")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateCountry([FromBody] InitializeCountryViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                info = new InitializeCountryViewModel();
                Validate(info);
            }

            // Retrieve model error.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            // Find the country whether it has been registered before or not.
            var countries = await _repositoryPlace.FindCountryAsync(null, info.Name,
                StringComparison.InvariantCultureIgnoreCase);

            // Country was created before.
            if (countries != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error = $"{Language.WarnRecordConflict}"
                });
            }

            // Initialize an instance of Country.
            var country = new Country();
            country.Name = info.Name;

            // Save country to database.
            await _repositoryPlace.InitializeCountryAsync(country);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Country = new
                {
                    country.Id,
                    country.Name
                }
            });
        }

        /// <summary>
        /// Find a country by using id and edit its information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/country")]
        [HttpPut]
        public async Task<HttpResponseMessage> EditCountry([FromUri] int id, [FromBody] InitializeCountryViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                info = new InitializeCountryViewModel();
                Validate(info);
            }

            // Retrieve model error.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #region Id validation

            // Find the country by using id.
            var countries = await _repositoryPlace.FindCountryAsync(id, null, null);

            // No record has been found or found record is not unique.
            if (countries == null || countries.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first queried result.
            var country = countries.FirstOrDefault();
            if (country == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Name validation

            // Check whether country name has been used before or not.
            var target = await _repositoryPlace.FindCountryAsync(null, info.Name, StringComparison.InvariantCultureIgnoreCase);

            // Record has been created before.
            if (target != null && target.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error = $"{Language.WarnRecordConflict}"
                });
            }

            #endregion

            // Update information of country
            country.Name = info.Name;

            try
            {
                // Save edited information to database.
                await _repositoryPlace.EditCountryAsync(country);

                // Send the edited information to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Country = new
                    {
                        country.Id,
                        country.Name
                    }
                });
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Filter countries by using given conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/country/filter")]
        [HttpPost]
        public async Task<HttpResponseMessage> FilerCountries([FromBody] FilterCountryViewModel filter)
        {
            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterCountryViewModel();
                Validate(filter);
            }

            // Invalid filter information.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            var results = await _repositoryPlace.FilterCountryAsync(filter);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        #endregion

        #region City

        /// <summary>
        /// Find a city by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/city")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCity([FromUri]int id)
        {
            // Using id to find country.
            var cities = await _repositoryPlace.FindCityAsync(id);

            // Records hasn't been found or not unique, treat the result as not found.
            if (cities == null || cities.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first result.
            var city = cities.FirstOrDefault();

            // Country is invalid.
            if (city == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Return the city information.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                City = new
                {
                    city.Id,
                    city.Name,
                    Country = new
                    {
                        Id = city.CountryId,
                        Name = city.CountryName
                    }
                }
            });
        }

        /// <summary>
        /// Initialize a city which belongs to a country asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/city")]
        [HttpPost]
        public async Task<HttpResponseMessage> InitializeCity([FromBody] InitializeCityViewModel info)
        {
            // Parameters haven't been initialized.
            if (info == null)
            {
                // Initialize and do validation.
                info = new InitializeCityViewModel();
                Validate(info);
            }

            // Invalid parameters detected.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #region Country validation

            // Find the country by using id.
            var countries = await _repositoryPlace.FindCountryAsync(info.Country, null, null);
            if (countries == null || countries.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnCountryNotFound}"
                });
            }

            // Find the first query country.
            var country = countries.FirstOrDefault();
            if (country == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnCountryNotFound}"
                });
            }

            #endregion

            // Initialize a city instance.
            var city = new City();
            city.Name = info.Name;
            city.CountryId = country.Id;
            city.CountryName = country.Name;

            try
            {
                // Save newly created city instance to database.
                await _repositoryPlace.InitializeCityAsync(city);

                _log.Info($"Created city[{city.Id}] : {city.Name} successfully");
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    City = new
                    {
                        city.Id,
                        city.Name,
                        Country = new
                        {
                            country.Id,
                            country.Name
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.InternalServerError}"
                });
            }
        }

        /// <summary>
        /// Filter and retrieve list of city with given information.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/city/filter")]
        [HttpPost]
        public async Task<HttpResponseMessage> FilterCity([FromBody] FilterCityViewModel filter)
        {
            // Filter model hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterCityViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            var results = await _repositoryPlace.FilterCityAsync(filter);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        #endregion
    }
}