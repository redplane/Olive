using System;
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
using Shared.Resources;
using Shared.ViewModels;

namespace OlivesAdministration.Controllers
{
    [Route("api/doctor")]
    public class DoctorController : ParentController
    {
        #region Properties

        /// <summary>
        ///     Instance of repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of DoctorController
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public DoctorController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        #endregion

        /// <summary>
        ///     Access role : Admin
        ///     Description : Retrieve a doctor by using specific id
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Get(RetrieveDoctorViewModel info)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
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
            var result = await _repositoryAccount.FindDoctorById(info.Id);

            // No result has been found.
            if (result == null || result.Count < 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // Not only one result has been retrieved.
            if (result.Count != 1)
                return Request.CreateResponse(HttpStatusCode.Conflict);

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        ///     Access role : Admin
        ///     Description : Create an user account with given information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Post(InitializeDoctorViewModel info)
        {
            #region ModelState validation

            // Invalid data validation.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            #region Identity card validation

            // Check whether this identity card is in use or not.
            var idAbleToRegister = await _repositoryAccount.IsDoctorAbleToRegister(null, info.IdentityCardNo);
            if (!idAbleToRegister)
            {
                // Throw error back to user.
                ModelState.AddModelError("Conflict", Language.DoctorExisted);

                // Respond back to client.
                return Request.CreateResponse(HttpStatusCode.Conflict, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Information initialization

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

            #endregion

            // Call repository function to create an account.
            var result = await Task.Run(() => _repositoryAccount.InitializePerson(doctor));

            // Transaction is failed. Tell client about the result.
            if (!result)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            // Tell the client, doctor has been added successfully.
            return Request.CreateResponse(HttpStatusCode.OK, doctor);
        }

        /// <summary>
        ///     Update a doctor information by using specific id.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Put(EditDoctorViewModel info)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            #region Retrieve doctor from database

            // Initialize doctor filter.
            var filterDoctorViewModel = new FilterDoctorViewModel();
            filterDoctorViewModel.Id = info.Id;

            // Retrieve doctors by using specific id.
            var results = await _repositoryAccount.FilterDoctorAsync(filterDoctorViewModel);

            // Invalid result.
            if (results == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // To doctors list.
            var doctors = results.Data;

            // No doctor has been found.
            if (doctors.Count < 1)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // More than one doctor has been found.
            if (doctors.Count > 1)
                return Request.CreateResponse(HttpStatusCode.Conflict);

            #endregion

            #region Identity card validation

            // Check whether this identity card is in use or not.
            var isIdentityCardAvailable = await _repositoryAccount.IsDoctorAbleToRegister(null, info.IdentityCardNo);
            if (!isIdentityCardAvailable)
            {
                // Tell the client, identity card is in use.
                ModelState.AddModelError("Conflict", Language.IdentityCardInUse);

                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            #endregion

            #region Information update

            var doctor = (Doctor) doctors[0];
            doctor.FirstName = info.FirstName;
            doctor.LastName = info.LastName;
            doctor.Birthday = info.Birthday ?? Values.MinimumSelectionTime;
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

            #region Result handling

            // By default, result comes back from repository is an object. 
            // It is need casting to IEnumerable<Node<string>> data type.
            var nodes = await _repositoryAccount.EditPersonAsync(info.Id, doctor);

            // No data comes back.
            if (nodes == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            // Retrieve the first result.
            var node = nodes.FirstOrDefault();

            // Invalid node.
            if (node == null || string.IsNullOrEmpty(node.Data))
                return Request.CreateResponse(HttpStatusCode.NoContent);

            // TODO: Modify information in Chat system.

            // Retrieve the updated information.
            doctor = JsonConvert.DeserializeObject<Doctor>(node.Data);

            // Return status OK to client to notify edition is successful.
            return Request.CreateResponse(HttpStatusCode.OK, doctor);

            #endregion
        }

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/doctor/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Roles.Admin})]
        public async Task<HttpResponseMessage> Filter(FilterDoctorViewModel info)
        {
            // Information hasn't been initialize.
            // By default, select all doctor without using any specific conditions.
            if (info == null)
                info = new FilterDoctorViewModel();

            // Set filter role to Doctor.
            info.Role = Roles.Doctor;

            // Retrieve result from server.
            var results = await _repositoryAccount.FilterDoctorAsync(info);

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }
    }
}