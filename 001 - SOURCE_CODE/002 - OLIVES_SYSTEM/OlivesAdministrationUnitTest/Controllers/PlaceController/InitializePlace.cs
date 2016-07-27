using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Test.Repositories;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Models;

namespace OlivesAdministration.Test.Controllers.PlaceController
{
    [TestClass]
    public class InitializePlace
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of Login with default settings.
        /// </summary>
        public InitializePlace()
        {
            // Initialize RepositoryAccount.
            _repositoryPlace = new RepositoryPlace();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof (FindPlace));

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
        /// Description: Find a place which doesn't exist in database.
        /// Expected : 404 will be thrown back.
        /// Action : Find the not existed place.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PlaceIsNotFound()
        {
            // Initialize a forgery place database.
            _repositoryPlace.Places = new List<Place>();
            
            // Find a place doesn't exist in database.
            var response = await _placeController.FindPlace(1);
            
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

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
            var response = await _placeController.InitializePlace(null);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Description: Request parameters are invalid.
        /// Expected : 400 will be thrown back.
        /// Action : No request parameter is sent.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CityIsntFilled()
        {
            var initializer = new InitializePlaceViewModel();
            
            // Find a place doesn't exist in database.
            var response = await _placeController.InitializePlace(initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Description: Request parameters are invalid.
        /// Expected : 400 will be thrown back.
        /// Action : No request parameter is sent.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CityMaxLengthReached()
        {
            var initializer = new InitializePlaceViewModel();
            for (var i = 0; i < 128; i++)
                initializer.City += i;

            // Find a place doesn't exist in database.
            var response = await _placeController.InitializePlace(initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Description: Request parameters are invalid.
        /// Expected : 400 will be thrown back.
        /// Action : No request parameter is sent.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CountryIsntFilled()
        {
            var initializer = new InitializePlaceViewModel();
            initializer.City = "City";
            initializer.Country = "";

            // Find a place doesn't exist in database.
            var response = await _placeController.InitializePlace(initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Description: Request parameters are invalid.
        /// Expected : 400 will be thrown back.
        /// Action : No request parameter is sent.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CountryMaxLengthReached()
        {
            var initializer = new InitializePlaceViewModel();
            initializer.City = "City";

            for (var i = 0; i < 100; i++)
                initializer.Country += i.ToString();

            // Find a place doesn't exist in database.
            var response = await _placeController.InitializePlace(initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Description: Place is duplicated, it cannot be initialized again.
        /// Expected : 409 will be thrown back.
        /// Action : Duplicate the initialization.
        /// </summary>
        /// <returns></returns>
        public async Task PlaceIsDuplicated()
        {
            // Forgery database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() {Id = 1, City = "1", Country = "1"});

            var initializer = new InitializePlaceViewModel();
            initializer.City = "1";
            initializer.Country = "1";

            // Call the initialization function.
            var response = await _placeController.InitializePlace(initializer);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);

        }
        #endregion
    }
}