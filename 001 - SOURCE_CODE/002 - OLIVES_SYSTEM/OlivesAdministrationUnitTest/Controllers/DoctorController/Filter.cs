using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Enumerations;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Repositories;
using OlivesAdministration.Test.Helpers;
using OlivesAdministration.Test.Repositories;
using OlivesAdministration.ViewModels.Filter;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;

namespace OlivesAdministration.Test.Controllers.DoctorController
{
    [TestClass]
    public class Filter
    {
        #region Initialization sector

        private OlivesAdministration.Controllers.DoctorController _doctorController;
        private IRepositoryAccountExtended _repositoryAccountExtended;
        private ILog _log;

        /// <summary>
        ///     Initialize context.
        /// </summary>
        private void InitializeContext()
        {
            // Data context initialiation.
            var oliveDataContext = new OliveDataContext();

            // Repositories initialization.
            _repositoryAccountExtended = new RepositoryAccountExtended(oliveDataContext);

            _log = LogManager.GetLogger(typeof(Get));
            _doctorController = new OlivesAdministration.Controllers.DoctorController(_repositoryAccountExtended, _log);
            EnvironmentHelper.Instance.InitializeController(_doctorController);
        }

        /// <summary>
        ///     Initialize function context.
        /// </summary>
        /// <param name="dataContext"></param>
        private void InitializeContext(IOliveDataContext dataContext)
        {
            _repositoryAccountExtended = new RepositoryAccountExtended(dataContext);
            _log = LogManager.GetLogger(typeof(Get));
            _doctorController = new OlivesAdministration.Controllers.DoctorController(_repositoryAccountExtended, _log);
            EnvironmentHelper.Instance.InitializeController(_doctorController);
        }

        #endregion

        #region Tests

        /// <summary>
        ///     Doctor filter isn't initialized.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DoctorFilterIsntInitialized()
        {
            InitializeContext();
            var result = await _doctorController.Filter(null);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        /// <summary>
        ///     Email contains invalid characters
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EmailIsInvalidLength()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Email = "";
            for (var i = 0; i < 100; i++)
                filter.Email += $"{i}";
            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     MinLastModified is 1900
        /// </summary>
        /// <returns></returns>
        public async Task InvalidMinLastModified()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MinLastModified = -2209014000000;
            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     MinLastModified must be smaller or equal than MaxLastModified.
        /// </summary>
        /// <returns></returns>
        public async Task MinLastModifiedGreaterMaxLastModified()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MinLastModified = 0;
            filter.MaxLastModified = -1;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     MaxLastModified is 1900
        /// </summary>
        /// <returns></returns>
        public async Task MaxLastModifiedInvalid()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MaxLastModified = -2209014000000;
            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Name length is more than 64
        /// </summary>
        /// <returns></returns>
        public async Task NameMaxLengthExceeded()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Name = "aa";
            for (var i = 0; i < FieldLength.FullNameMaxLength + 1; i++)
                filter.Name += "b";

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Max birthday is less than 1916.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinBirthdayLessThan1916()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MinBirthday = -2209014000000;
            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Max birthday is less than 1916.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MaxBirthdayLessThan1916()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MinBirthday = -2209014000000;
            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Min birthday is greater than max birthday.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinBirthdayGreaterMaxBirthday()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MinBirthday = 0;
            filter.MaxBirthday = -1;
            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Invalid gender.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InvalidGender()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Gender = (Gender?) -1;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Min created is smaller than the allowed value.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinCreatedLessThanMinimum()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MinCreated = -2209014000000;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Max created is smaller than the allowed value.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MaxCreatedLessThanMinimum()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MaxCreated = -2209014000000;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Min created is larger than min created.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinCreatedGreaterMaxCreated()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MaxCreated = 1;
            filter.MinCreated = 2;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     City name max length exceeded.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CityMaxLengthExceeded()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.City = "a";
            for (var i = 0; i < FieldLength.CityNameMaxLength + 10; i++)
                filter.City += "a";

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Country name max length exceeded.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CountryMaxLengthExceeded()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Country = "a";
            for (var i = 0; i < FieldLength.CountryNameMaxLength + 10; i++)
                filter.Country += "a";

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Specialty is invalid.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InvalidSpecialty()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Specialty = 0;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }


        /// <summary>
        ///     Min rank is smaller than the allowed value.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinRankLessThanMinimum()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MinRank = -1;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Max rank is smaller than the allowed value.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MaxRankLessThanMinimum()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MaxRank = -1;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Min created is larger than min created.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MinRankGreaterThanMaxRank()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.MaxRank = 1;
            filter.MinRank = 2;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Sort direction is invalid.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InvalidSortDirection()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Direction = (SortDirection) (-1);

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Invalid sort properties
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InvalidSortedProperty()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Sort = (FilterDoctorSort) (-1);

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Page index is invalid.
        /// </summary>
        /// <returns></returns>
        public async Task PageIndexIsInvalid()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Page = -1;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Record less than one.
        /// </summary>
        /// <returns></returns>
        public async Task RecordLessThanOne()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Records = 0;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Record more than twenty.
        /// </summary>
        /// <returns></returns>
        public async Task RecordMoreThanTwenty()
        {
            InitializeContext();
            var filter = new FilterDoctorViewModel();
            filter.Records = 21;

            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(filter);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Filter doctor successfully.
        /// </summary>
        /// <returns></returns>
        public async Task FilterDoctorSuccessfully()
        {
            var dataContext = new Shared.Repositories.OliveDataContext();
            await EnvironmentHelper.Instance.InitializePlaces(dataContext.Context, 10);
            await EnvironmentHelper.Instance.InitializeSpecialties(dataContext.Context, 10);
            await EnvironmentHelper.Instance.InitializeDoctor(dataContext.Context, 10);

            InitializeContext(dataContext);

            var filter = new FilterDoctorViewModel();
            _doctorController.Validate(filter);
            var result = await _doctorController.Filter(null);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion
    }
}