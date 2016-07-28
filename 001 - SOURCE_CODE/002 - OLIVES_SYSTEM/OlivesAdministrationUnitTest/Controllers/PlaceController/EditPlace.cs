using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Test.Repositories;
using OlivesAdministration.ViewModels.Edit;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Models;

namespace OlivesAdministration.Test.Controllers.PlaceController
{
    [TestClass]
    public class EditPlace
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of Login with default settings.
        /// </summary>
        public EditPlace()
        {
            // Initialize RepositoryAccount.
            _repositoryPlace = new RepositoryPlace();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof(EditPlace));

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
            var response = await _placeController.ModifyPlace(0, null);

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
            var modifier = new EditPlaceViewModel();
            for (var i = 0; i < 128; i++)
                modifier.City += i;

            // Do validation.
            _placeController.Validate(modifier);

            // Find a place doesn't exist in database.
            var response = await _placeController.ModifyPlace(1, modifier);

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
            var modifier = new EditPlaceViewModel();
            modifier.City = "City";

            for (var i = 0; i < 100; i++)
                modifier.Country += i.ToString();

            // Find a place doesn't exist in database.
            var response = await _placeController.ModifyPlace(1, modifier);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public async Task PlaceIsNotFound()
        {
            // Forgery database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() { Id = 1, City = "1", Country = "1" });

            var modifier = new EditPlaceViewModel();
            modifier.City = "2";
            modifier.Country = "1";

            var response = await _placeController.ModifyPlace(0, modifier);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
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
            _repositoryPlace.Places.Add(new Place() { Id = 1, City = "1", Country = "1" });

            var modifier = new EditPlaceViewModel();
            modifier.City = "1";
            modifier.Country = "1";

            // Call the initialization function.
            var response = await _placeController.ModifyPlace(1, modifier);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        /// <summary>
        /// Description: Place meets .
        /// Expected : 200 will be thrown back.
        /// Action : Duplicate the initialization.
        /// </summary>
        /// <returns></returns>
        public async Task PlaceModifiedSuccessfully()
        {
            // Forgery database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() { Id = 1, City = "1", Country = "1" });

            var modifier = new EditPlaceViewModel();
            modifier.City = "2";
            modifier.Country = "2";

            // Call the initialization function.
            var response = await _placeController.ModifyPlace(1, modifier);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Description: Added place has the same city as database but different country .
        /// Expected : 200 will be thrown back.
        /// </summary>
        /// <returns></returns>
        public async Task PlaceModidiedSameCitySuccessfully()
        {
            // Forgery database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() { Id = 1, City = "1", Country = "1" });

            var modifier = new EditPlaceViewModel();
            modifier.City = "1";
            modifier.Country = "2";

            // Call the initialization function.
            var response = await _placeController.ModifyPlace(1, modifier);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Description: Added place has the same country as database but different city.
        /// Expected : 200 will be thrown back.
        /// </summary>
        /// <returns></returns>
        public async Task PlaceModidiedSameCountrySuccessfully()
        {
            // Forgery database.
            _repositoryPlace.Places = new List<Place>();
            _repositoryPlace.Places.Add(new Place() { Id = 1, City = "1", Country = "1" });

            var modifier = new EditPlaceViewModel();
            modifier.City = "2";
            modifier.Country = "1";

            // Call the initialization function.
            var response = await _placeController.ModifyPlace(1, modifier);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion
    }
}