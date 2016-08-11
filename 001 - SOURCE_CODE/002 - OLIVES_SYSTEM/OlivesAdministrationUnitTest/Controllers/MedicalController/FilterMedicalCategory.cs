using System;
using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Repositories;
using OlivesAdministration.Test.Helpers;
using OlivesAdministration.Test.Repositories;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repositories;
using Shared.Services;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Test.Controllers.MedicalController
{
    [TestClass]
    public class FilterMedicalCategory
    {
        #region Initialization sector

        private OlivesAdministration.Controllers.MedicalCategoryController _medicalController;
        private IRepositoryMedicalCategory _repositoryMedicalCategory;
        private ITimeService _timeService;
        private ILog _log;

        /// <summary>
        /// Initialize context.
        /// </summary>
        private void InitializeContext()
        {
            // Data context initialiation.
            var oliveDataContext = new Repositories.OliveDataContext();
            
            // Initialize time service.
            _timeService = new TimeService();

            // Repositories initialization.
            _repositoryMedicalCategory = new RepositoryMedicalCategory(oliveDataContext);
            
            _log = LogManager.GetLogger(typeof(InitializeMedicalCategory));
            _medicalController = new OlivesAdministration.Controllers.MedicalCategoryController(_repositoryMedicalCategory, _timeService, _log);
            EnvironmentHelper.Instance.InitializeController(_medicalController);
        }

        /// <summary>
        /// Initialize function context.
        /// </summary>
        /// <param name="dataContext"></param>
        private void InitializeContext(IOliveDataContext dataContext)
        {
            _repositoryMedicalCategory = new RepositoryMedicalCategory(dataContext);
            _log = LogManager.GetLogger(typeof(InitializeMedicalCategory));
            _timeService = new TimeService();
            _medicalController = new OlivesAdministration.Controllers.MedicalCategoryController(_repositoryMedicalCategory, _timeService, _log);
            EnvironmentHelper.Instance.InitializeController(_medicalController);
        }

        #endregion

        #region Tests
        
        /// <summary>
        /// Category name max length reached.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryNameMaxLengthReached()
        {
            InitializeContext();
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "a";
            for (var i = 0; i < Values.MaxMedicalCategoryNameLength + 10; i++)
                initializer.Name += "a";

            _medicalController.Validate(initializer);
            var result = await _medicalController.ModifyMedicalCategory(1, initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MinCreated is smaller than the allowed value.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinCreatedSmallerThanMinimumValue()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.MinCreated = -1704178800000;
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MaxCreated is smaller than the allowed value.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MaxCreatedSmallerThanMinimumValue()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.MaxCreated = -1704178800000;
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MinCreated > MaxCreated
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinCreatedGreaterThanMaxCreated()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.MinCreated = 2;
            filter.MaxCreated = 1;
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MinLastModified.Year is 1916
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinLastModifiedSmallerAllowMin()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.MinLastModified = _timeService.DateTimeUtcToUnix(new DateTime(1916, 31, 12));
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MaxLastModified.Year is 1916
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MaxLastModifiedSmallerAllowMin()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.MaxLastModified = _timeService.DateTimeUtcToUnix(new DateTime(1916, 31, 12));
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MedicalCategorySort is less than 0
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategorySortMinimumIsInvalid()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.Sort = (MedicalCategoryFilterSort) (-1);
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MedicalCategorySort is more than 4
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategorySortMaximumIsInvalid()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.Sort = (MedicalCategoryFilterSort)(10);
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MedicalCategorySort is less than 0
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategorySortDirectionIsInvalid()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.Direction = (SortDirection)(-1);
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// MedicalCategorySort is more than 4
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryDirectionMaximumIsInvalid()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.Direction = (SortDirection)(10);
            _medicalController.Validate(filter);

            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// Filter.Page less than 0
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task FilterMedicalCategoryPageLessThanZero()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.Page = -1;
            
            _medicalController.Validate(filter);
            var result = await _medicalController.FilterCategories(filter);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        /// Filter.Records less than Zero.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task FilterMedicalRecordsLessThanZero()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.Records = -1;

            _medicalController.Validate(filter);
            var result = await _medicalController.FilterCategories(filter);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task FilterMedicalRecordsMoreThanTwenty()
        {
            InitializeContext();
            var filter = new FilterMedicalCategoryViewModel();
            filter.Records = FieldLength.RecordMax + 1;
            _medicalController.Validate(filter);
            var result = await _medicalController.FilterCategories(filter);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task FilterMedicalRecordCategorySuccessful()
        {
            var dataContext = new Repositories.OliveDataContext();
            await EnvironmentHelper.Instance.InitializeMedicalCategories(dataContext.Context, 10);
            InitializeContext(dataContext);

            var filter = new FilterMedicalCategoryViewModel();
            _medicalController.Validate(filter);
            var result = await _medicalController.FilterCategories(filter);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
        #endregion
    }
}