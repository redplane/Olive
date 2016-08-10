using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Repositories;
using OlivesAdministration.Test.Helpers;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repositories;
using Shared.Services;

namespace OlivesAdministration.Test.Controllers.MedicalController
{
    [TestClass]
    public class InitializeMedicalCategory
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
            _medicalController = new OlivesAdministration.Controllers.MedicalCategoryController(_repositoryMedicalCategory, _timeService, _log);
            EnvironmentHelper.Instance.InitializeController(_medicalController);
        }

        #endregion

        #region Tests

        /// <summary>
        /// Category name shouldn't be blank
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryNameIsBlank()
        {
            InitializeContext();
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = null;

            _medicalController.Validate(initializer);
            var result = await _medicalController.InitializeMedicalCategory(initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        }

        /// <summary>
        /// Medical category maximum length exceeded.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryNameMaxLengthExceeded()
        {
            InitializeContext();
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "";

            for (var i = 0; i < Values.MaxMedicalCategoryNameLength + 10; i++)
                initializer.Name += "a";

            _medicalController.Validate(initializer);
            var result = await _medicalController.InitializeMedicalCategory(initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);

        }

        /// <summary>
        /// Initialize medical category which is already in server.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryDuplicated()
        {
            var dataContext = new Repositories.OliveDataContext();
            await EnvironmentHelper.Instance.InitializeMedicalCategories(dataContext.Context, 10);

            InitializeContext(dataContext);
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "1";

            _medicalController.Validate(initializer);
            var result = await _medicalController.InitializeMedicalCategory(initializer);

            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }

        /// <summary>
        /// No duplicate result is found, insertion is successful.
        /// </summary>
        /// <returns></returns>
        public async Task MedicalCategoryInitializedSuccessfully()
        {
            var dataContext = new Repositories.OliveDataContext();
            
            InitializeContext(dataContext);
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = "1";

            _medicalController.Validate(initializer);
            var result = await _medicalController.InitializeMedicalCategory(initializer);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion
    }
}