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

namespace OlivesAdministration.Test.Controllers.DoctorController
{
    [TestClass]
    public class Get
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
        ///     Account is not a doctor.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AccountIsNotDoctor()
        {
            #region Account initialization

            var dataContext = new OliveDataContext();
            var person = new Person();
            person.Id = 1;
            person.Email = $"doctor@gmail.com";
            person.Password = "doctor199x";
            person.FirstName = "firstName";
            person.LastName = "lastName";
            person.FullName = person.FirstName + " " + person.LastName;
            person.Gender = 0;
            person.Role = (byte) Role.Admin;
            person.Created = 1;

            dataContext.Context.People.Add(person);
            await dataContext.Context.SaveChangesAsync();

            #endregion

            InitializeContext(dataContext);
            var result = await _doctorController.Get(1);

            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        /// <summary>
        ///     Account is not found in database.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DoctorIsNotFound()
        {
            #region Account initialization

            var dataContext = new OliveDataContext();
            var person = new Person();
            person.Id = 1;
            person.Email = $"doctor@gmail.com";
            person.Password = "doctor199x";
            person.FirstName = "firstName";
            person.LastName = "lastName";
            person.FullName = person.FirstName + " " + person.LastName;
            person.Gender = 0;
            person.Role = (byte) Role.Admin;
            person.Created = 1;

            dataContext.Context.People.Add(person);
            await dataContext.Context.SaveChangesAsync();

            #endregion

            InitializeContext(dataContext);
            var result = await _doctorController.Get(2);

            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        /// <summary>
        ///     Account is not a doctor.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DoctorIsInDatabase()
        {
            #region Data initialization

            var dataContext = new OliveDataContext();
            await EnvironmentHelper.Instance.InitializeSpecialties(dataContext.Context, 10);
            await EnvironmentHelper.Instance.InitializePlaces(dataContext.Context, 10);
            await EnvironmentHelper.Instance.InitializeDoctor(dataContext.Context, 10);

            InitializeContext(dataContext);
            EnvironmentHelper.Instance.InitializeController(_doctorController);

            #endregion

            var result = await _doctorController.Get(3);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion
    }
}