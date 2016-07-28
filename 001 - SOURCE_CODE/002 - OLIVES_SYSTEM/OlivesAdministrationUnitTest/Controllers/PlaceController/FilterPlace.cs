using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Test.Repositories;
using Shared.Models;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Test.Controllers.PlaceController
{
    [TestClass]
    public class FilterPlace
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of Login with default settings.
        /// </summary>
        public FilterPlace()
        {
            // Initialize RepositoryAccount.
            _repositoryPlace = new RepositoryPlace();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof(FilterPlace));

            // Initialize a fake controller.
            _placeController = new OlivesAdministration.Controllers.PlaceController(_repositoryPlace, log);

            // Override HttpRequest to do testing.
            var configuration = new HttpConfiguration();
            var request = new HttpRequestMessage();
            _placeController.Request = request;
            _placeController.Request.Properties["MS_HttpConfiguration"] = configuration;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Medical controller.
        /// </summary>
        private readonly OlivesAdministration.Controllers.PlaceController _placeController;

        /// <summary>
        ///     Repository medical which simulates function of RepositoryMedical to test controller.
        /// </summary>
        private readonly RepositoryPlace _repositoryPlace;

        #endregion

        #region Methods

        /// <summary>
        /// Description: Request parameters are invalid.
        /// Expected : 400 will be thrown back.
        /// Action : No request parameter is sent.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestParametersAreInvalid()
        {
            // Find a place doesn't exist in database.
            var response = await _placeController.FilterPlace(null);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Country max length reached.
        /// </summary>
        /// <returns></returns>
        public async Task CityMaxLengthReached()
        {
            // Filter initialiation.
            var filter = new FilterPlaceViewModel();

            // City name max length reached.
            filter.City = "";
            filter.Country = "1";
            for (var i = 0; i < 100; i++)
                filter.City += "a";

            // City maximum length reached, 400 should be thrown.
            var response = await _placeController.FilterPlace(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Page index max length reached.
        /// </summary>
        /// <returns></returns>
        public async Task CountryMaxLengthReached()
        {
            // Filter initialiation.
            var filter = new FilterPlaceViewModel();

            // City name max length reached.
            filter.City = "1";
            filter.Country = "";
            for (var i = 0; i < 100; i++)
                filter.Country += "a";

            // City maximum length reached, 400 should be thrown.
            var response = await _placeController.FilterPlace(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Page index is invalid.
        /// </summary>
        /// <returns></returns>
        public async Task PageIndexSmallerThanZero()
        {
            // Filter initialiation.
            var filter = new FilterPlaceViewModel();

            // City name max length reached.
            filter.Page = -1;

            // City maximum length reached, 400 should be thrown.
            var response = await _placeController.FilterPlace(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Record index is invalid.
        /// </summary>
        /// <returns></returns>
        public async Task RecordsSmallerThanZero()
        {
            // Filter initialiation.
            var filter = new FilterPlaceViewModel();

            // City name max length reached.
            filter.Records = -1;

            // City maximum length reached, 400 should be thrown.
            var response = await _placeController.FilterPlace(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Record index is invalid.
        /// </summary>
        /// <returns></returns>
        public async Task RecordsGreaterThanTwenty()
        {
            // Filter initialiation.
            var filter = new FilterPlaceViewModel();

            // City name max length reached.
            filter.Records = 21;

            // City maximum length reached, 400 should be thrown.
            var response = await _placeController.FilterPlace(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public async Task FilterPlaceIsSuccessful()
        {
            // Forgery database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() { Id = 1, City = "1", Country = "1" });

            // Filter initialization.
            var filter = new FilterPlaceViewModel();

            var response = await _repositoryPlace.FilterPlacesAsync(filter);
            Assert.AreEqual(1, response.Total);
        }

        #endregion
    }
}