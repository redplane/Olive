using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotnetSignalR.Attributes;
using DotnetSignalR.ViewModels;
using Neo4jClient;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models.Nodes;
using Shared.ViewModels;
using InitializeDoctorViewModel = DotnetSignalR.ViewModels.InitializeDoctorViewModel;

namespace DotnetSignalR.Controllers
{
    public class DoctorController : ParentController
    {
        /// <summary>
        ///     Instance of repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Initialize an instance of DoctorController
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public DoctorController(IRepositoryAccount repositoryAccount) : base(repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        /// <summary>
        ///     Access role : Admin
        ///     Description : Retrieve a doctor by using specific id
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<ActionResult> Get(GetDoctorViewModel info)
        {
            var response = new ResponseViewModel();

            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int) HttpStatusCode.BadRequest;

                // Errors list construction.
                response.Errors = RetrieveValidationErrors(ModelState);
                return Json(null);
            }

            #endregion

            #region Information initialization

            // Only filter by specific GUID and role.
            var filter = new FilterDoctorViewModel();
            filter.Id = info.Id;
            filter.IdentityCardNo = info.IdentityCardNo;
            filter.Page = 0;
            filter.Records = 1;

            #endregion

            #region Results handling

            // Retrieve filtered result asynchronously.
            var result = await _repositoryAccount.FindDoctor(filter);

            // No result has been found.
            if (result == null)
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            // Initialize response from server.
            response.Data = result;

            #endregion

            Response.StatusCode = (int) HttpStatusCode.OK;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Access role : Admin
        ///     Description : Create an user account with given information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<ActionResult> Post(InitializeDoctorViewModel info)
        {
            #region ModelState validation

            if (!ModelState.IsValid)
            {
                var response = new ResponseViewModel();
                response.Errors = RetrieveValidationErrors(ModelState);

                return Json(response);
            }

            #endregion

            #region Identity card validation
            
            // Check whether this identity card is in use or not.
            var idAbleToRegister = await _repositoryAccount.IsDoctorAbleToRegister(null, info.IdentityCardNo);
            if (!idAbleToRegister)
            {
                var response = new ResponseViewModel();
                var errors = new List<string>();
                errors.Add(ErrorCodes.DoctorIdentityConflict);
                response.Data = errors;

                Response.StatusCode = (int) HttpStatusCode.Conflict;
                return Json(response);
            }

            #endregion

            // TODO: Create a person on chat system.
            // Initialize an instance of Doctor.
            var doctor = new Doctor();
            doctor.Id = Guid.NewGuid().ToString("N");
            doctor.FirstName = info.FirstName;
            doctor.LastName = info.LastName;
            doctor.Birthday = info.Birthday;
            doctor.Gender = info.Gender;
            doctor.Email = info.Email;
            doctor.Password = info.Password;
            doctor.Phone = info.Phone;
            doctor.Created = DateTime.Now.Ticks;
            doctor.Role = Roles.Doctor;

            if (info.Address != null)
            {
                doctor.AddressLongitude = info.Address.Longitude;
                doctor.AddressLatitude = info.Address.Latitude;
            }

            doctor.IdentityCardNo = info.IdentityCardNo;
            doctor.Specialization = info.Specialization;

            // Call repository function to create an account.
            var result = await Task.Run(() => _repositoryAccount.InitializePerson(doctor));

            // Transaction is failed. Tell client about the result.
            if (!result)
            {
                Response.StatusCode = (int) HttpStatusCode.Conflict;
                return Json(null);
            }

            Response.StatusCode = (int) HttpStatusCode.OK;
            return Json(null);
        }

        /// <summary>
        ///     Update a doctor information by using specific id.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<ActionResult> Put(EditDoctorViewModel info)
        {
            // Initialize a response form which server will respond to client.
            var response = new ResponseViewModel();

            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int) HttpStatusCode.BadRequest;

                // Errors list construction.
                response.Errors = RetrieveValidationErrors(ModelState);
                return Json(response);
            }

            #endregion

            #region Retrieve doctor from database

            // Initialize doctor filter.
            var filterDoctorViewModel = new FilterDoctorViewModel();
            filterDoctorViewModel.Id = info.Id;

            // Retrieve doctors by using specific id.
            var results = await _repositoryAccount.FilterDoctorAsync(filterDoctorViewModel);

            // Invalid result.
            if (results == null)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Json(null);
            }

            // To doctors list.
            var doctors = results.ToList();

            // No doctor has been found.
            if (doctors.Count < 1)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Json(null);
            }

            // More than one doctor has been found.
            if (doctors.Count > 1)
            {
                // Treat the result as conflict on server.
                Response.StatusCode = (int) HttpStatusCode.Conflict;
                return Json(null);
            }

            #endregion

            #region Identity card validation

            // Check whether this identity card is in use or not.
            var isIdentityCardAvailable = await _repositoryAccount.IsDoctorAbleToRegister(null, info.IdentityCardNo);
            if (!isIdentityCardAvailable)
            {
                var errors = new List<string>();
                errors.Add(ErrorCodes.DoctorIdentityConflict);
                response.Data = errors;

                Response.StatusCode = (int) HttpStatusCode.Conflict;
                return Json(response);
            }

            #endregion

            #region Information update

            var doctor = doctors[0];
            doctor.FirstName = info.FirstName;
            doctor.LastName = info.LastName;
            doctor.Birthday = info.Birthday;
            doctor.Gender = info.Gender;

            if (!string.IsNullOrEmpty(info.Password))
                doctor.Password = info.Password;

            if (info.Address != null)
            {
                doctor.AddressLongitude = info.Address.Longitude;
                doctor.AddressLatitude = info.Address.Latitude;
            }

            doctor.Phone = info.Phone;
            doctor.Money = info.Money;
            doctor.Status = info.Status;
            doctor.Specialization = info.Specialization;
            doctor.Rank = info.Rank;

            #endregion

            // By default, result comes back from repository is an object. 
            // It is need casting to IEnumerable<Node<string>> data type.
            var rawResult = await _repositoryAccount.UpdatePersonAsync(info.Id, doctor);

            // No data comes back.
            if (rawResult == null)
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Json(null);
            }

            // Retrieve the first result.
            var resultNode = ((IEnumerable<Node<string>>) rawResult).FirstOrDefault();

            // Invalid node.
            if (resultNode == null || string.IsNullOrEmpty(resultNode.Data))
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Json(null);
            }

            // TODO: Modify information in Chat system.
            // Retrieve the updated information.
            doctor = JsonConvert.DeserializeObject<Doctor>(resultNode.Data);

            // Update the response data.
            response.Data = doctor;

            // Return status OK to client to notify edition is successful.
            Response.StatusCode = (int) HttpStatusCode.OK;
            return Json(response);
        }

        [HttpPost]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<ActionResult> Filter(FilterDoctorViewModel info)
        {
            var results = await _repositoryAccount.FilterDoctorAsync(info);
            Response.StatusCode = (int) HttpStatusCode.OK;
            return Json(results);
        }
    }
}