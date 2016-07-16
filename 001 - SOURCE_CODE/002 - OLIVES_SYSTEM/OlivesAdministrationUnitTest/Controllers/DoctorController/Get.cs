using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Models;
using OlivesAdministration.Test.Repositories;
using Shared.Enumerations;
using Shared.Models;
using Shared.ViewModels;

namespace OlivesAdministration.Test.Controllers.DoctorController
{
    [TestClass]
    public class Get
    {
        #region Properties

        /// <summary>
        /// Admin controller.
        /// </summary>
        private readonly OlivesAdministration.Controllers.DoctorController _doctorController;

        /// <summary>
        /// Repository account which simulates function of RepositoryAccount to test controller.
        /// </summary>
        private readonly RepositoryAccount _repositoryAccount;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize an instance of Login with default settings.
        /// </summary>
        public Get()
        {
            // Initialize RepositoryAccount.
            _repositoryAccount = new RepositoryAccount();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof(Get));

            // Initialize fake application setting instance.
            var applicationSetting = new ApplicationSetting();

            // Initialize a fake controller.
            _doctorController = new OlivesAdministration.Controllers.DoctorController(_repositoryAccount, log, applicationSetting);

            // Override HttpRequest to do testing.
            var configuration = new HttpConfiguration();
            var request = new HttpRequestMessage();
            _doctorController.Request = request;
            _doctorController.Request.Properties["MS_HttpConfiguration"] = configuration;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Doctor is invalid.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DoctorNotExist()
        {
            var doctor = new Doctor();
            doctor.Id = 1;
            doctor.City = "City";
            doctor.Country = "Country";
            doctor.Money = 10;
            doctor.PlaceId = 1;
            doctor.Rank = 1;

            // Forging data.
            _repositoryAccount.Doctors = new List<Doctor>();
            _repositoryAccount.Doctors.Add(doctor);
            
            // Get an invalid doctor.
            var response = await _doctorController.Get(2);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        #endregion
    }
}

