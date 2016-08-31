using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlivesAdministration.Interfaces;
using OlivesAdministration.Repositories;
using OlivesAdministration.Test.Helpers;
using OlivesAdministration.Test.Repositories;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Services;
using Shared.ViewModels;

namespace OlivesAdministration.Test.Controllers.AdminController
{
    [TestClass]
    public class Login
    {
        #region Initialization sector

        private OlivesAdministration.Controllers.AdminController _adminController;
        private IRepositoryAccountExtended _repositoryAccountExtended;
        private ILog _log;
        private ITimeService _timeService;

        #endregion

        #region Initialization

        /// <summary>
        ///     Initialize context.
        /// </summary>
        private void InitializeContext()
        {
            // Data context initialiation.
            var oliveDataContext = new OliveDataContext();

            // Repositories initialization.
            _repositoryAccountExtended = new RepositoryAccountExtended(oliveDataContext);
            _timeService = new TimeService();
            _log = LogManager.GetLogger(typeof(Login));
            _adminController = new OlivesAdministration.Controllers.AdminController(_repositoryAccountExtended, _log,
                _timeService);
            EnvironmentHelper.Instance.InitializeController(_adminController);
        }

        /// <summary>
        ///     Initialize function context.
        /// </summary>
        /// <param name="dataContext"></param>
        private void InitializeContext(IOliveDataContext dataContext)
        {
            _repositoryAccountExtended = new RepositoryAccountExtended(dataContext);
            _log = LogManager.GetLogger(typeof(Login));
            _adminController = new OlivesAdministration.Controllers.AdminController(_repositoryAccountExtended, _log,
                _timeService);
            EnvironmentHelper.Instance.InitializeController(_adminController);
        }

        /// <summary>
        ///     Initialize function context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task InitializeAdminAccounts(OlivesHealthEntities context)
        {
            for (var i = 0; i < 3; i++)
            {
                // General information.
                var person = new Person();
                person.Email = $"admin{i}@gmail.com";
                person.Password = "00F2BA2AA3E9AD96D400A7CA60C5FDA0";
                person.FirstName = $"AF[{i}]";
                person.LastName = $"AL[{i}]";
                person.FullName = person.FirstName + " " + person.LastName;
                person.Gender = 0;
                person.Role = (byte) Role.Admin;
                person.Created = 1;

                if (i == 0)
                    person.Status = (byte) StatusAccount.Active;
                else if (i == 1)
                    person.Status = (byte) StatusAccount.Pending;
                else if (i == 2)
                    person.Status = (byte) StatusAccount.Inactive;

                context.People.Add(person);
            }

            await context.SaveChangesAsync();
        }

        #endregion

        #region Tests

        /// <summary>
        ///     No information has been input to login board.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task BlankLoginInformaion()
        {
            // Initialize test environment.
            InitializeContext();

            var loginViewModel = new LoginViewModel();
            _adminController.Validate(loginViewModel);
            var result = await _adminController.LoginAsync(loginViewModel);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Email is correct but password is missing.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PasswordIsMissing()
        {
            // Initialize test environment.
            InitializeContext();

            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = "admin26@gmail.com";

            _adminController.Validate(loginViewModel);
            var result = await _adminController.LoginAsync(loginViewModel);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     Password is filled but email is not.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EmailIsMissing()
        {
            // Initialize test environment.
            InitializeContext();

            var loginViewModel = new LoginViewModel();
            loginViewModel.Password = "admin199x";

            _adminController.Validate(loginViewModel);
            var result = await _adminController.LoginAsync(loginViewModel);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        /// <summary>
        ///     All information is filled, but wrong
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EmailIsInvalid()
        {
            var dataContext = new OliveDataContext();
            InitializeContext(dataContext);
            await InitializeAdminAccounts(dataContext.Context);

            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = "admin10@gmail.com";
            loginViewModel.Password = "password";
            
            _adminController.Validate(loginViewModel);

            var result = await _adminController.LoginAsync(loginViewModel);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        /// <summary>
        ///     Admin account is pending.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AccountIsPending()
        {
            var dataContext = new OliveDataContext();
            InitializeContext(dataContext);
            await InitializeAdminAccounts(dataContext.Context);

            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = "admin1@gmail.com";
            loginViewModel.Password = "password";
            
            _adminController.Validate(loginViewModel);

            var result = await _adminController.LoginAsync(loginViewModel);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        /// <summary>
        ///     Account is currently deactivated.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AccountIsDeactivated()
        {
            var dataContext = new OliveDataContext();
            InitializeContext(dataContext);
            await InitializeAdminAccounts(dataContext.Context);

            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = "admin2@gmail.com";
            loginViewModel.Password = "password";

            var validationResult = await _adminController.LoginAsync(loginViewModel);
            _adminController.Validate(loginViewModel);

            var result = await _adminController.LoginAsync(loginViewModel);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        /// <summary>
        ///     Login is valid because email and password are correct.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task LoginSuccessful()
        {
            var dataContext = new OliveDataContext();   
            InitializeContext(dataContext);
            await InitializeAdminAccounts(dataContext.Context);

            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = "admin0@gmail.com";
            loginViewModel.Password = "admin199x";

            _adminController.Validate(loginViewModel);

            var result = await _adminController.LoginAsync(loginViewModel);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion
    }
}