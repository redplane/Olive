using System;
using System.Collections;
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

        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {
            // Whether request comes from valid people or not.
            var isInvalidRole = await IsInValidRoleAsync(Roles.Admin);
            if (isInvalidRole != HttpStatusCode.OK)
            {
                Response.StatusCode = (int)isInvalidRole;
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            // TODO: Implement modelstate checking.

            // Only filter by specific GUID and role.
            var filter = new FilterDoctorViewModel();
            filter.Id = id;
            filter.Role = Roles.Doctor;
            filter.Page = 0;
            filter.Records = 1;

            // Retrieve filtered result asynchronously.
            var results = await _repositoryAccount.FilterDoctorAsync(filter);

            // No result has been found.
            if (results == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            // Retrieve the first result.
            var result = results.FirstOrDefault();
            if (result == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            // Password shouldn't be shown.
            result.Password = "";

            // Initialize response from server.
            var response = new ResponseViewModel();
            response.Data = result;

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Access role : Admin
        /// Description : Create an user account with given information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(CreateDoctorViewModel info)
        {
            // Request doesn't come from a person who has specific roles.
            var isInValidRole = await IsInValidRoleAsync(Roles.Admin);
            if (isInValidRole != HttpStatusCode.OK)
            {
                Response.StatusCode = (int)isInValidRole;
                return Json(null);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                var response = new ResponseViewModel();
                response.Errors = RetrieveValidationErrors(ModelState);

                return Json(response);
            }

            // Whether email has been used before or not.
            var doctor = await _repositoryAccount.GetPersonExistAsync(info.Email, null, null);
            if (doctor != null)
            {
                var errors = new List<string>();
                errors.Add(ErrorCodes.UserHasAlreadyExisted);

                var response = new ResponseViewModel();
                response.Errors = errors;

                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return Json(response);
            }

            doctor = new Doctor();
            doctor.Id = Guid.NewGuid().ToString();
            doctor.FirstName = info.FirstName;
            doctor.LastName = info.LastName;
            doctor.Birthday = info.Birthday;
            doctor.Gender = info.Gender;
            doctor.Address = info.Address;
            doctor.Email = info.Email;
            doctor.Password = info.Password;
            doctor.Phone = info.Phone;
            doctor.Money = info.Money;
            doctor.Created = DateTime.Now.Ticks;
            doctor.Role = Roles.Doctor;

            // Call repository function to create an account.
            await _repositoryAccount.CreatePersonAsync(doctor);
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(null);
        }

        [HttpPost]
        public async Task<ActionResult> Put(EditDoctorViewModel info)
        {
            // Invalid role.
            // TODO: Uncomment this.
            var roleResult = await IsInValidRoleAsync(Roles.Admin);
            if (roleResult != HttpStatusCode.OK)
            {
                Response.StatusCode = (int)roleResult;
                return Json(null);
            }

            // Initialize a response form which server will respond to client.
            var response = new ResponseViewModel();

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // Errors list construction.
                response.Errors = RetrieveValidationErrors(ModelState);
                return Json(null);
            }

            // Todo: Continue the implementation.
            var doctor = new Doctor();
            
            // By default, result comes back from repository is an object. 
            // It is need casting to IEnumerable<Node<string>> data type.
            var rawResult = await _repositoryAccount.UpdatePersonAsync("999", doctor);

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

            doctor = JsonConvert.DeserializeObject<Doctor>(resultNode.Data);
            var a = 1;
            return Json(null);
        }
    }
}