using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
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