using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Models;
using OlivesAdministration.Test.Repositories;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OlivesAdministration.Test.Controllers.MedicalController
{
    [TestClass]
    public class InitializeMedicalCategory
    {
        #region Properties

        /// <summary>
        /// Medical controller.
        /// </summary>
        private readonly OlivesAdministration.Controllers.MedicalController _medicalController;

        /// <summary>
        /// Repository medical which simulates function of RepositoryMedical to test controller.
        /// </summary>
        private readonly RepositoryMedical _repositoryMedical;

        /// <summary>
        /// Instance for logger.
        /// </summary>
        private readonly ILog _log;
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize an instance of Login with default settings.
        /// </summary>
        public InitializeMedicalCategory()
        {
            // Initialize RepositoryAccount.
            _repositoryMedical = new RepositoryMedical();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof(InitializeMedicalCategory));

            // Initialize a fake controller.
            _medicalController = new OlivesAdministration.Controllers.MedicalController(_repositoryMedical, log);

            // Override HttpRequest to do testing.
            var configuration = new HttpConfiguration();
            var request = new HttpRequestMessage();
            _medicalController.Request = request;
            _medicalController.Request.Properties["MS_HttpConfiguration"] = configuration;

        }

        #endregion

        [TestMethod]
        #region Methods
        public async Task BadRequestNullableViewModel()
        {
            var response = await _medicalController.InitializeMedicalCategory(null);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);

        }

        
        [TestMethod]
        public async Task ConflicRequest()
        {
            var medicalCategory = new MedicalCategory();
            medicalCategory.Name = "mc1";
            // Initialize the category into database.
            //medicalCategory = await _repositoryMedical.InitializeMedicalCategoryAsync(medicalCategory);
            _repositoryMedical.Categories.Clear();
            _repositoryMedical.Categories.Add(medicalCategory);

            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "mc1";

            var response = await _medicalController.InitializeMedicalCategory(initializer);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Conflict);

        }

        [TestMethod]
        public async Task StatusOK()
        {
            var medicalCategory = new MedicalCategory();
            medicalCategory.Name = "mc1";
            // Initialize the category into database.
            //medicalCategory = await _repositoryMedical.InitializeMedicalCategoryAsync(medicalCategory);
            _repositoryMedical.Categories.Clear();
            _repositoryMedical.Categories.Add(medicalCategory);

            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "mc2";

            var response = await _medicalController.InitializeMedicalCategory(initializer);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task StatusOK_SuccessfullyAddedNewCategory()
        {
            var medicalCategory = new MedicalCategory();
            medicalCategory.Name = "mc1";
            // Initialize the category into database.
            //medicalCategory = await _repositoryMedical.InitializeMedicalCategoryAsync(medicalCategory);
            _repositoryMedical.Categories.Clear();
            _repositoryMedical.Categories.Add(medicalCategory);

            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "mc2";

            int COUNT_BEFORE = _repositoryMedical.Categories.Count;

            var response = await _medicalController.InitializeMedicalCategory(initializer);

            int COUNT_AFTER = _repositoryMedical.Categories.Count;
            Assert.IsTrue(--COUNT_AFTER == COUNT_BEFORE);
        }

        #endregion
    }

}