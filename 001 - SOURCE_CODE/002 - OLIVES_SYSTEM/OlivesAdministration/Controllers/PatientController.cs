using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Controllers
{
    [Route("api/patient")]
    public class PatientController : ApiParentController
    {
        #region Dependency injections

        /// <summary>
        ///     Repository account DI
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public PatientController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        #endregion

        /// <summary>
        ///     Find a patient by using a specific id.
        /// </summary>
        /// <param name="id">Id of patient</param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Get(int id)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
                // Because model is invalid. Treat this as invalid request.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            // Retrieve list of patients.
            var patients = await _repositoryAccount.FindPatientAsync(id);

            #region Result handling

            // Reponse error initialization.
            var responseError = new ResponseErrror();
            responseError.Errors = new List<string>();

            // No patient has been found.
            if (patients.Count < 1)
            {
                responseError.Errors.Add(Language.InvalidPatient);
                return Request.CreateResponse(HttpStatusCode.NotFound, responseError);
            }
            // More than one result has been retrieved.
            if (patients.Count != 1)
            {
                responseError.Errors.Add(Language.RecordIsNotUnique);
                return Request.CreateResponse(HttpStatusCode.Conflict, responseError);
            }

            #endregion

            // Respond to client.
            return Request.CreateResponse(HttpStatusCode.OK, new {User = patients[0]});
        }

        /// <summary>
        ///     Filter patient by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/patient/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterPatientViewModel filter)
        {
            #region ModelState validation

            // Filter hasn't been initialized . Initialize it.
            if (filter == null)
            {
                filter = new FilterPatientViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
                // Because model is invalid. Treat this as invalid request.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            // Filter patient by using specific conditions.
            var results = await _repositoryAccount.FilterPatientAsync(filter);

            // Result is null. Initialize an empty list.
            if (results.Users == null)
                results.Users = new List<PatientViewModel>();

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }
    }
}