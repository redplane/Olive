using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels;
using Shared.Enumerations;
using Shared.Interfaces;
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {AccountRole.Admin})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            #region ModelState validation

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                // Because model is invalid. Treat this as invalid request.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Results handling

            // Retrieve filtered result asynchronously.
            var results = await _repositoryAccount.FindDoctorAsync(id);

            // No result has been found.
            if (results == null || results.Count < 1)
            {
                ModelState.AddModelError("FindResult", Language.DoctorDoesNotExist);
                return Request.CreateResponse(HttpStatusCode.NotFound, RetrieveValidationErrors(ModelState));
            }


            // Not only one result has been retrieved.
            if (results.Count != 1)
            {
                ModelState.AddModelError("FindResult", Language.FindResultConflict);
                return Request.CreateResponse(HttpStatusCode.Conflict, RetrieveValidationErrors(ModelState));
            }

            // Retrieve the first result.
            var result = results.FirstOrDefault();

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, new {User = result});
        }

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/doctor/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {AccountRole.Admin})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterDoctorViewModel info)
        {
            #region ModelState validation

            //// Request parameters haven't been initialized.
            //if (info == null)
            //    return Request.CreateResponse(HttpStatusCode.BadRequest, Language.InvalidRequestParameters);

            // Invalid data validation.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            #region Result handling

            // Retrieve result from server.
            var result = await _repositoryAccount.FilterDoctorAsync(info);

            // Result hasn't been initialized. Initialize it.
            if (result.Users == null)
                result.Users = new List<DoctorViewModel>();

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}