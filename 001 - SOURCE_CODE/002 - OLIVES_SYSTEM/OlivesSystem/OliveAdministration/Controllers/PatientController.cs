using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotnetSignalR.ViewModels;
using Neo4jClient;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models.Nodes;
using Shared.ViewModels;

namespace DotnetSignalR.Controllers
{
    public class PatientController : ParentController
    {
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public PatientController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }
        
        /// <summary>
        /// Find a patient by using a specific id.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<ActionResult> Get(FindPatientViewModel info)
        {
            // Initialize response.
            var response = new ResponseViewModel();

            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // Errors list construction.
                response.Errors = RetrieveValidationErrors(ModelState);
                return Json(response);
            }

            #endregion

            // Retrieve list of patients.
            var patients = await _repositoryAccount.FindPatientById(info.Id);

            #region Result handling

            // No patient has been found.
            if (patients.Count < 1)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            // More than one result has been retrieved.
            if (patients.Count != 1)
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);

            #endregion

            // Respond to client.
            response.Data = patients[0];
            return Json(response);
        }
        
        /// <summary>
        /// Create a patient with specific information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<ActionResult> Post(InitializePatientViewModel info)
        {

            // Initialize response.
            var response = new ResponseViewModel();

            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // Errors list construction.
                response.Errors = RetrieveValidationErrors(ModelState);
                return Json(response);
            }

            #endregion

            #region Account initialization check

            // Access to db to check whether patient can be registered or not.
            var isPatientAbleToInitialize = await _repositoryAccount.IsPatientAbleToCreated(null, info.Email);

            // Patient cannot be initialized.
            if (!isPatientAbleToInitialize)
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return Json(response);
            }

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
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);

            response.Data = patient;
            Response.StatusCode = (int) HttpStatusCode.OK;

            return Json(response);
        }

        public async Task<ActionResult> Put(FindPatientViewModel patient, InitializePatientViewModel info)
        {
            // Initialize response.
            var response = new ResponseViewModel();
            if (!TryUpdateModel(info, null, null, new[] { "Email" })) // fourth parameter is an array of properties (by name) that are excluded
            {
                response.Errors = RetrieveValidationErrors(ModelState);
                // updated and validated correctly!
                return Json(null);
            }
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // Errors list construction.
                response.Errors = RetrieveValidationErrors(ModelState);
                return Json(response);
            }

            return Json(null);
            #endregion
            
            // Retrieve patient from database.
            var patients = await _repositoryAccount.FindPatientById(patient.Id);

            #region Result handling

            // No patient has been found.
            if (patients.Count < 1)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            // More than one result has been retrieved.
            if (patients.Count != 1)
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);

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
            var rawResult = await _repositoryAccount.EditPersonAsync(patient.Id, patients[0]);

            // No data comes back.
            if (rawResult == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null);
            }

            // Retrieve the first result.
            var resultNode = ((IEnumerable<Node<string>>)rawResult).FirstOrDefault();

            // Invalid node.
            if (resultNode == null || string.IsNullOrEmpty(resultNode.Data))
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null);
            }

            // TODO: Modify information in Chat system.
            // Retrieve the updated information.
            var responseResult = JsonConvert.DeserializeObject<Doctor>(resultNode.Data);

            // Update the response data.
            response.Data = responseResult;

            #endregion

            // Return status OK to client to notify edition is successful.
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(response);
        }
    }
}