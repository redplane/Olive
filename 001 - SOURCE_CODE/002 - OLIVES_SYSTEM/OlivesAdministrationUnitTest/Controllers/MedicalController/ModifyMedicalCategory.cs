using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Controllers;
using OlivesAdministration.Test.Helpers;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Repositories;
using Shared.Services;
using OliveDataContext = OlivesAdministration.Test.Repositories.OliveDataContext;

namespace OlivesAdministration.Test.Controllers.MedicalController
{
    [TestClass]
    public class ModifyMedicalCategory
    {
        #region Initialization sector

        private MedicalCategoryController _medicalController;
        private IRepositoryMedicalCategory _repositoryMedicalCategory;
        private ITimeService _timeService;
        private ILog _log;

        /// <summary>
        ///     Initialize context.
        /// </summary>
        private void InitializeContext()
        {
            // Data context initialiation.
            var oliveDataContext = new OliveDataContext();

            // Initialize time service.
            _timeService = new TimeService();

            // Repositories initialization.
            _repositoryMedicalCategory = new RepositoryMedicalCategory(oliveDataContext);

            _log = LogManager.GetLogger(typeof(InitializeMedicalCategory));
            _medicalController = new MedicalCategoryController(_repositoryMedicalCategory, _timeService, _log);
            EnvironmentHelper.Instance.InitializeController(_medicalController);
        }

        /// <summary>
        ///     Initialize function context.
        /// </summary>
        /// <param name="dataContext"></param>
        private void InitializeContext(IOliveDataContext dataContext)
        {
            _repositoryMedicalCategory = new RepositoryMedicalCategory(dataContext);
            _log = LogManager.GetLogger(typeof(InitializeMedicalCategory));
            _timeService = new TimeService();
            _medicalController = new MedicalCategoryController(_repositoryMedicalCategory, _timeService, _log);
            EnvironmentHelper.Instance.InitializeController(_medicalController);
        }

        #endregion

        #region Tests

        /// <summary>
        ///     Category name shouldn't be blank
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryNameIsBlank()
        {
            InitializeContext();
            var initializer = new InitializeMedicalCategoryViewModel();
            initializer.Name = null;

            _medicalController.Validate(initializer);
            var result = await _medicalController.ModifyMedicalCategory(1, initializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Category name max length reached.
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
        ///     Category doesn't exist in database.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryDoesntExist()
        {
            var dataContext = new OliveDataContext();
            await EnvironmentHelper.Instance.InitializeMedicalCategories(dataContext.Context, 10);
            InitializeContext(dataContext);

            var modifier = new InitializeMedicalCategoryViewModel();
            modifier.Name = "12";

            var result = await _medicalController.ModifyMedicalCategory(11, modifier);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        /// <summary>
        ///     Medical name is already in database.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryNameConflict()
        {
            var dataContext = new OliveDataContext();
            await EnvironmentHelper.Instance.InitializeMedicalCategories(dataContext.Context, 10);
            InitializeContext(dataContext);

            var modifier = new InitializeMedicalCategoryViewModel();
            modifier.Name = "0";

            var result = await _medicalController.ModifyMedicalCategory(1, modifier);
            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }

        /// <summary>
        ///     Modification is succesful.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MedicalCategoryModifySuccessfully()
        {
            var dataContext = new OliveDataContext();
            await EnvironmentHelper.Instance.InitializeMedicalCategories(dataContext.Context, 10);
            InitializeContext(dataContext);

            var modifier = new InitializeMedicalCategoryViewModel();
            modifier.Name = "10";

            var result = await _medicalController.ModifyMedicalCategory(1, modifier);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion
    }
}