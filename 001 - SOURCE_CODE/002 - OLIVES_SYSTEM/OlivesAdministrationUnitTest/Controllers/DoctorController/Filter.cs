using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Models;
using OlivesAdministration.Test.Repositories;
using Shared.Constants;
using Shared.Models;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Test.Controllers.DoctorController
{
    [TestClass]
    public class Filter
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of Login with default settings.
        /// </summary>
        public Filter()
        {
            // Initialize RepositoryAccount.
            _repositoryAccount = new RepositoryAccount();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof (Get));

            // Initialize fake application setting instance.
            var applicationSetting = new ApplicationSetting();

            // Initialize a fake controller.
            _doctorController = new OlivesAdministration.Controllers.DoctorController(_repositoryAccount, log,
                applicationSetting);

            // Override HttpRequest to do testing.
            var configuration = new HttpConfiguration();
            var request = new HttpRequestMessage();
            _doctorController.Request = request;
            _doctorController.Request.Properties["MS_HttpConfiguration"] = configuration;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Admin controller.
        /// </summary>
        private readonly OlivesAdministration.Controllers.DoctorController _doctorController;

        /// <summary>
        ///     Repository account which simulates function of RepositoryAccount to test controller.
        /// </summary>
        private readonly RepositoryAccount _repositoryAccount;

        #endregion

        #region Methods

        [TestMethod]
        public async Task InvalidRequestParameters()
        {
            var response = await _doctorController.Filter(null);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Min money must be smaller than or equal to max money.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinMoneyGreaterThanMaxMoney()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.MinMoney = 1;
            filter.MaxMoney = 0;

            // Do validation to retrieve errors.
            _doctorController.Validate(filter);

            // Do the filter.
            var response = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// City name max length should not be exceeded.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CityNameMaxLengthReached()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.City = "aaaa";

            // Enlarge the city name,
            for (var i = 0; i < FieldLength.CityNameMaxLength; i++)
                filter.City += "a";

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the response.
            var response = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Country name max length should not be exceeded.
        /// </summary>
        /// <returns></returns>
        public async Task CountryNameMaxLengthExceeded()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.Country = "aaaa";

            // Enlarge the country name.
            for (var i = 0; i < FieldLength.CountryNameMaxLength; i++)
                filter.Country += "a";

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the reponse.
            var response = await _doctorController.Filter(filter);

            // Compare the result and actual result.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Specialty index is smaller than 1.
        /// </summary>
        /// <returns></returns>
        public async Task SpecialtyIndexSmallerThanOne()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.Specialty = 0;

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the reponse.
            var response = await _doctorController.Filter(filter);

            // Compare the result and actual result.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// MinRank is greater than MaxRank.
        /// </summary>
        /// <returns></returns>
        public async Task MinRankGreaterThanMaxRank()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.MinRank = 2;
            filter.MaxRank = 1;

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the reponse.
            var response = await _doctorController.Filter(filter);

            // Compare the result and actual result.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// MinRank smaller than the allowed minimum.
        /// </summary>
        /// <returns></returns>
        public async Task MinRankSmallerThanMinimum()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.MinRank = 0;

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the reponse.
            var response = await _doctorController.Filter(filter);

            // Compare the result and actual result.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// MinRank larger than the allowed maximum.
        /// </summary>
        /// <returns></returns>
        public async Task MinRankLargerThanMaximum()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.MinRank = 6;

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the reponse.
            var response = await _doctorController.Filter(filter);

            // Compare the result and actual result.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// MinRank smaller than the allowed minimum.
        /// </summary>
        /// <returns></returns>
        public async Task MaxRankSmallerThanMinimum()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.MaxRank = 0;

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the reponse.
            var response = await _doctorController.Filter(filter);

            // Compare the result and actual result.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// MinRank larger than the allowed maximum.
        /// </summary>
        /// <returns></returns>
        public async Task MaxRankLargerThanMaximum()
        {
            // Filter initialization.
            var filter = new FilterDoctorViewModel();
            filter.MaxRank = 6;

            // Validate the property.
            _doctorController.Validate(filter);

            // Retrieve the reponse.
            var response = await _doctorController.Filter(filter);

            // Compare the result and actual result.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        #endregion
    }
}