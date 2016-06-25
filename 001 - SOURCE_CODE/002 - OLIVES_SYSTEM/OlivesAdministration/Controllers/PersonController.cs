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
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;

namespace OlivesAdministration.Controllers
{
    [Route("api/person")]
    public class PersonController : ApiParentController
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

        [Route("api/person/status")]
        [HttpPost]
        [OlivesAuthorize(new[] {AccountRole.Admin})]
        public async Task<HttpResponseMessage> Status(EditStatusViewModel info)
        {
            #region Model validation

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            // Find the person from database using unique identity.
            var person = await _repositoryAccount.FindPersonAsync(info.Id, null, null, null, null);

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
            var result = await _repositoryAccount.EditPersonStatusAsync(info.Id, info.Status);

            // Error happens while changing account status.
            if (result == null)
            {
                // Response error construction.
                var responseError = new ResponseErrror();
                responseError.Errors = new List<string>();

                responseError.Errors.Add(Language.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseError);
            }

            if ((AccountStatus)info.Status == AccountStatus.Active)
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Message = new[] {Language.AccountHasBeenActivated}
                });

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Message = new[] {Language.AccountHasBeenDisabled}
            });
        }

        [Route("api/person/statistic/status")]
        [HttpPost]
        [OlivesAuthorize(new[] {AccountRole.Admin})]
        public async Task<HttpResponseMessage> Statistic([FromBody] StatusSummaryViewModel info)
        {
            #region Model validation

            // Information hasn't been initialized. Initialize and validate it.
            if (info == null)
            {
                info = new StatusSummaryViewModel();
                Validate(info);
            }

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            // Find the person from database using unique identity.
            var summaryResult = await _repositoryAccount.SummarizePersonRole(info.Role);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                ActiveAccounts = summaryResult.Where(x => (AccountStatus)x.Status == AccountStatus.Active).Sum(x => x.Total),
                PendingAccounts = summaryResult.Where(x => (AccountStatus)x.Status == AccountStatus.Pending).Sum(x => x.Total),
                DeactiveAccounts = summaryResult.Where(x => (AccountStatus)x.Status == AccountStatus.Inactive).Sum(x => x.Total),
                Total = summaryResult.Sum(x => x.Total)
            });
        }

        #endregion
    }
}