﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models.Nodes;
using Shared.ViewModels;

namespace OlivesAdministration.Controllers
{
    [Route("api/patient")]
    public class PatientController : ParentController
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
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Get([FromUri] FindPatientViewModel info)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
                // Because model is invalid. Treat this as invalid request.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            // Retrieve list of patients.
            var patients = await _repositoryAccount.FindPatientById(info.Id);

            #region Result handling

            // No patient has been found.
            if (patients.Count < 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // More than one result has been retrieved.
            if (patients.Count != 1)
                return Request.CreateResponse(HttpStatusCode.Conflict);

            #endregion

            // Respond to client.
            return Request.CreateResponse(HttpStatusCode.OK, patients[0]);
        }

        /// <summary>
        ///     Create a patient with specific information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializePatientViewModel info)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            #region Account initialization check

            // Access to db to check whether patient can be registered or not.
            var isPatientAbleToInitialize = await _repositoryAccount.IsPatientAbleToCreated(null, info.Email);

            // Patient cannot be initialized.
            if (!isPatientAbleToInitialize)
                return Request.CreateResponse(HttpStatusCode.Conflict);

            #endregion

            #region Data initialization

            var patient = new Patient();
            patient.Id = Guid.NewGuid().ToString("N");
            patient.FirstName = info.FirstName;
            patient.LastName = info.LastName;
            patient.Birthday = info.Birthday;
            patient.Gender = info.Gender;
            patient.Email = info.Email;
            patient.Password = info.Password;
            patient.Phone = info.Phone;
            patient.Money = info.Money;
            patient.Created = DateTime.Now.Ticks;
            patient.Role = Roles.Patient;
            patient.Status = AccountStatus.Active;
            patient.Height = info.Height;
            patient.Weight = info.Weight;
            patient.Anamneses = info.Anamneses;

            #endregion

            // Initialize patient information into db.
            var result = _repositoryAccount.InitializePerson(patient);
            if (!result)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            return Request.CreateResponse(HttpStatusCode.OK, patient);
        }

        /// <summary>
        ///     Edit patient information.
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Put([FromUri] FindPatientViewModel patient,
            [FromBody] InitializePatientViewModel info)
        {
            #region ModelState validation

            // Email cannot be edited, therefore, it doesn't required.
            ModelState.Remove("Email");

            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);

            #endregion

            // Retrieve patient from database.
            var patients = await _repositoryAccount.FindPatientById(patient.Id);

            #region Result handling

            // No patient has been found.
            if (patients.Count < 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // More than one result has been retrieved.
            if (patients.Count != 1)
                return Request.CreateResponse(HttpStatusCode.Conflict);

            #endregion

            // Todo : Modify information on chat system.
            patients[0].FirstName = info.FirstName;
            patients[0].LastName = info.LastName;
            patients[0].Birthday = info.Birthday;
            patients[0].Gender = info.Gender;
            patients[0].Password = info.Password;
            patients[0].Phone = info.Phone;
            patients[0].Money = info.Money;
            patients[0].Height = info.Height;
            patients[0].Weight = info.Weight;
            patients[0].Anamneses = info.Anamneses;

            #region Update result handling

            // By default, result comes back from repository is an object. 
            // It is need casting to IEnumerable<Node<string>> data type.
            var nodes = await _repositoryAccount.EditPersonAsync(patient.Id, patients[0]);

            // No data comes back.
            if (nodes == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // Retrieve the first result.
            var node = nodes.FirstOrDefault();

            // Invalid node.
            if (node == null || string.IsNullOrEmpty(node.Data))
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // TODO: Modify information in Chat system.
            // Retrieve the updated information.
            patients[0] = JsonConvert.DeserializeObject<Patient>(node.Data);

            #endregion

            // Return status OK to client to notify edition is successful.
            return Request.CreateResponse(HttpStatusCode.OK, patients[0]);
        }

        /// <summary>
        ///     Disable patient account.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        [Route("api/patient/status")]
        [HttpGet]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Status([FromUri] ModifyPatientStatusViewModel patient)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);

            #endregion

            // Retrieve patients from database.
            var patients = await _repositoryAccount.FindPatientById(patient.Id);

            #region Result handling

            // No patient has been found.
            if (patients.Count < 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // More than one result has been retrieved.
            if (patients.Count != 1)
                return Request.CreateResponse(HttpStatusCode.Conflict);

            #endregion

            // Modify account status to disabled.
            var result = await _repositoryAccount.ModifyAccountStatus(patient.Id, patient.Status);

            return Request.CreateResponse(!result ? HttpStatusCode.NotModified : HttpStatusCode.OK);
        }

        [Route("api/patient/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterPatientViewModel filter)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
                // Because model is invalid. Treat this as invalid request.
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);

            #endregion

            // Filter patient by using specific conditions.
            var results = await _repositoryAccount.FilterPatientAsync(filter);

            // No record has been retrieved.
            if (results == null || results.Total < 1 || results.Data.Count < 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }
    }
}