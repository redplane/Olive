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
    public class FindPlace
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of Login with default settings.
        /// </summary>
        public FindPlace()
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
        /// Description: Find a place which doesn't exist in database.
        /// Expected : 404 will be thrown back.
        /// Action : Find the not existed place.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PlaceIsNotFoundDataExist()
        {
            // Initialize a forgery place database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() {City = "1", Country = "1", Id = 1});
            _repositoryPlace.Places.Add(new Place() { City = "2", Country = "2", Id = 2 });
            _repositoryPlace.Places.Add(new Place() { City = "3", Country = "2", Id = 3 });

            // Find a place doesn't exist in database.
            var response = await _placeController.FindPlace(4);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Description: Find a place which exists in database.
        /// Expected : 200 will be thrown back.
        /// Action : Find the existed place.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PlaceIsFound()
        {
            // Initialize a forgery place database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() { City = "1", Country = "1", Id = 1 });
            _repositoryPlace.Places.Add(new Place() { City = "2", Country = "2", Id = 2 });
            _repositoryPlace.Places.Add(new Place() { City = "3", Country = "2", Id = 3 });

            // Find a place doesn't exist in database.
            var response = await _placeController.FindPlace(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }


        #endregion
    }
}