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

namespace OlivesAdministration.Test.Controllers.MedicalController
{
    [TestClass]
    public class InitializeMedicalCategory
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of Login with default settings.
        /// </summary>
        public InitializeMedicalCategory()
        {
            // Initialize RepositoryAccount.
            _repositoryMedicalCategory = new RepositoryMedicalCategory();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof (InitializeMedicalCategory));

            // Initialize a fake controller.
            _medicalController = new OlivesAdministration.Controllers.MedicalController(_repositoryMedicalCategory, log);

            // Override HttpRequest to do testing.
            var configuration = new HttpConfiguration();
            var request = new HttpRequestMessage();
            _medicalController.Request = request;
            _medicalController.Request.Properties["MS_HttpConfiguration"] = configuration;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Medical controller.
        /// </summary>
        private readonly OlivesAdministration.Controllers.MedicalController _medicalController;

        /// <summary>
        ///     Repository medical which simulates function of RepositoryMedical to test controller.
        /// </summary>
        private readonly RepositoryMedicalCategory _repositoryMedicalCategory;
        
        #endregion
        
        #region Methods

        /// <summary>
        /// Description : This test shows that bad request is thrown when input paramters are invalid.
        /// Condition : Request parameters are invalid.
        /// Action : No parameter is sent to function.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InitializationThrowsBadRequest()
        {
            var response = await _medicalController.InitializeMedicalCategory(null);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }
        
        /// <summary>
        /// Description : This test shows that conflict error (409) is thrown when user tries to initialize existed record into database.
        /// Condition : Data exists in database.
        /// Action: Initialize a duplicated record.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InitializationIsConflict()
        {
            // Clear the forgery database.
            _repositoryMedicalCategory.MedicalCategories = new List<MedicalCategory>();

            // Initialize the first record.
            var medicalCategory = new MedicalCategory();
            medicalCategory.Id = 1;
            medicalCategory.Name = "mc1";
            _repositoryMedicalCategory.MedicalCategories.Add(medicalCategory);

            // Initialize the second record whose name is the same as the first one's.
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "mc1";

            var response = await _medicalController.InitializeMedicalCategory(initializer);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Conflict);
        }

        /// <summary>
        /// Description: This test shows that the data initialization is successful.
        /// Condition : Data doesn't exist in database.
        /// Action : Add a new data.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InitializationIsSuccessful()
        {
            // Clear the forgery database.
            _repositoryMedicalCategory.MedicalCategories = new List<MedicalCategory>();

            // Initialize the first record.
            var medicalCategory = new MedicalCategory();
            medicalCategory.Name = "mc1";
            _repositoryMedicalCategory.MedicalCategories.Add(medicalCategory);

            // Initialize the second record whose name is different from the first one.
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "mc2";

            // Status check.
            var response = await _medicalController.InitializeMedicalCategory(initializer);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        /// <summary>
        /// Description: This test shows that the data initialization is successful. Records number should be increased by one.
        /// Condition : Data doesn't exist in database.
        /// Action : Add a new data.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InitializationSuccessfulDataCounter()
        {
            // Clear the forgery database.
            _repositoryMedicalCategory.MedicalCategories = new List<MedicalCategory>();
            
            // Initialize the first record.
            var medicalCategory = new MedicalCategory();
            medicalCategory.Id = 1;
            medicalCategory.Name = "mc1";
            _repositoryMedicalCategory.MedicalCategories.Add(medicalCategory);

            // Count the original data records number in database. 
            var original = _repositoryMedicalCategory.MedicalCategories.Count;

            // Initialize the second record.
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "mc2";
            
            // Initialize the second record.
            await _medicalController.InitializeMedicalCategory(initializer);

            // Count the number of records of modified list.
            var modified = _repositoryMedicalCategory.MedicalCategories.Count;

            // Compare the 2 number.
            Assert.IsTrue(original == modified - 1);
        }

        #endregion
    }
}