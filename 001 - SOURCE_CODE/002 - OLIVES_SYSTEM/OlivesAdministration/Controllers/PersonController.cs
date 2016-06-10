using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using System.Web.WebPages.Scope;
using Newtonsoft.Json;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models;
using Shared.Models.Nodes;
using Shared.Resources;
using Shared.ViewModels;

namespace OlivesAdministration.Controllers
{
    [Route("api/person")]
    public class PersonController : ParentController
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
        public PersonController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        #endregion

        #region Methods

        [Route("api/person/change_status")]
        [HttpPut]
        [OlivesAuthorize(new[] { Roles.Admin })]
        public async Task<HttpResponseMessage> ModifyStatus(ModifyStatusViewModel info)
        {
            #region Model validation

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            // Find the person from database using unique identity.
            var person = await _repositoryAccount.FindPerson(info.Id);

            // No person has been found.
            if (person == null)
            {
                // Response error construction.
                var responseError = new ResponseErrror();
                responseError.Errors = new List<string>();

                responseError.Errors.Add(Language.InvalidPerson);
                return Request.CreateResponse(HttpStatusCode.NotFound, responseError);
            }

            // Change account status and retrieve the process result.
            var result = await _repositoryAccount.ModifyPersonStatus(info.Id, info.Status);

            if (!result)
            {
                // Response error construction.
                var responseError = new ResponseErrror();
                responseError.Errors = new List<string>();

                responseError.Errors.Add(Language.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseError);
            }

            if (info.Status == AccountStatus.Active)
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Message = new[] { Language.AccountHasBeenActivated }
                });
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Message = new[] { Language.AccountHasBeenDisabled }
            });
        }

        #endregion
    }
}