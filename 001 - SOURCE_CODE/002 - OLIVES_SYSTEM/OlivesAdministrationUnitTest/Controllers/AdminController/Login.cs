using System.Collections.Generic;
using System.Diagnostics;
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

namespace OlivesAdministration.Test.Controllers.AdminController
{
    [TestClass]
    public class Login
    {
        #region Properties

        /// <summary>
        /// Admin controller.
        /// </summary>
        private readonly OlivesAdministration.Controllers.AdminController _adminController;

        /// <summary>
        /// Repository account which simulates function of RepositoryAccount to test controller.
        /// </summary>
        private readonly RepositoryAccount _repositoryAccount;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize an instance of Login with default settings.
        /// </summary>
        public Login()
        {
            // Initialize RepositoryAccount.
            _repositoryAccount = new RepositoryAccount();

            // Initialize fake log instance.
            var log = LogManager.GetLogger(typeof(Login));

            // Initialize fake application setting instance.
            var applicationSetting = new ApplicationSetting();

            // Initialize a fake controller.
            _adminController = new OlivesAdministration.Controllers.AdminController(_repositoryAccount, applicationSetting, log);

            // Override HttpRequest to do testing.
            var configuration = new HttpConfiguration();
            var request = new HttpRequestMessage();
            _adminController.Request = request;
            _adminController.Request.Properties["MS_HttpConfiguration"] = configuration;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Login throws bad request because submited information isn't initialized.
        /// </summary>
        [TestMethod]
        public async Task BadRequest()
        {
            // Call the login function and retrieve the responded message.
            var response = await _adminController.Login(null);

            // Compare the responded status with the HttpStatusCode.BadRequest.
            // If they're equal, function runs correctly.
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Login is failed due to invalid information.
        /// Status: Deactive
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InvalidLoginInformation()
        {
            // Forging a deactivated account.
            var account = new Person();
            account.Id = 1;
            account.Email = "deactivated@gmail.com";
            account.Password = "password199x";
            account.Role = (byte)Role.Admin;
            account.Status = (byte)StatusAccount.Inactive;

            // Add the fake person to list.
            _repositoryAccount.People = new List<Person>();
            _repositoryAccount.People.Clear();
            _repositoryAccount.People.Add(account);

            // Initialize login request parameters.
            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = account.Email;
            loginViewModel.Password = account.Password;

            // Call the login function.
            // Response will be 404 because no valid account is found.
            var response = await _adminController.Login(loginViewModel);
            Debug.WriteLine(response);

            // Compare the result thrown back.
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }


        /// <summary>
        /// Login is failed due to invalid information.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task BlankEmail()
        {
            // Forging a deactivated account.
            var account = new Person();
            account.Id = 1;
            account.Email = "";
            account.Password = "password199x";
            account.Role = (byte)Role.Admin;
            account.Status = (byte)StatusAccount.Inactive;

            // Add the fake person to list.
            _repositoryAccount.People = new List<Person>();
            _repositoryAccount.People.Clear();
            _repositoryAccount.People.Add(account);

            // Initialize login request parameters.
            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = null;
            loginViewModel.Password = account.Password;

            // Call the login function.
            // Response will be NotFound because no valid account is found.
            // In Reality, the respond might be diffrent.
            var response = await _adminController.Login(loginViewModel);
            Debug.WriteLine(response);

            // Compare the result thrown back.
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Login is failed due to invalid information.
        /// Role: Not Admin
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AdminAndNormalAccountDuplicateEmail()
        {
            // Forging a deactivated account.
            var account = new Person();
            account.Id = 1;
            account.Email = "admin@admin.com";
            account.Password = "password199x";
            account.Role = (byte)Role.Admin;
            account.Status = (byte)StatusAccount.Active;

            var account2 = new Person();
            account.Id = 2;
            account.Email = "admin@admin.com";
            account.Password = "password199x";
            account.Role = (byte)Role.Doctor;
            account.Status = (byte)StatusAccount.Active;

            var account3 = new Person();
            account.Id = 3;
            account.Email = "admin@admin.com";
            account.Password = "password199x";
            account.Role = (byte)Role.Patient;
            account.Status = (byte)StatusAccount.Active;

            // Add the fake person to list.
            _repositoryAccount.People = new List<Person>();
            _repositoryAccount.People.Clear();
            _repositoryAccount.People.Add(account);

            // Initialize login request parameters.
            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = account.Email;
            loginViewModel.Password = account.Password;

            // Call the login function.
            // Response will be NotFound because no valid account is found.
            // In Reality, the respond might be diffrent.
            var response = await _adminController.Login(loginViewModel);
            Debug.WriteLine(response);

            // Compare the result thrown back.
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Conflict); // might be Forbidden.
        }

        /// <summary>
        /// Login is failed due to invalid information.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task StatusOK()
        {
            // Forging a deactivated account.
            var account = new Person();
            account.Id = 1;
            account.Email = "admin@admin.admin";
            account.Password = "password199x";
            account.Role = (byte)Role.Admin;
            account.Status = (byte)StatusAccount.Inactive;

            // Add the fake person to list.
            _repositoryAccount.People = new List<Person>();
            _repositoryAccount.People.Clear();
            _repositoryAccount.People.Add(account);

            // Initialize login request parameters.
            var loginViewModel = new LoginViewModel();
            loginViewModel.Email = account.Email;
            loginViewModel.Password = account.Password;

            // Call the login function.
            // Response will be NotFound because no valid account is found.
            // In Reality, the respond might be diffrent.
            var response = await _adminController.Login(loginViewModel);
            Debug.WriteLine(response);

            // Compare the result thrown back.
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
        #endregion
    }
}

