using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Admin.Attributes;
using Olives.Admin.Interfaces;
using OlivesAdministration.ViewModels.Edit;
using OlivesAdministration.ViewModels.Statistic;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.Admin.Controllers
{
    [Route("api/person")]
    public class PersonController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="log"></param>
        public PersonController(IRepositoryAccountExtended repositoryAccountExtended, ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _log = log;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account DI
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Instance of log.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for modifying personal status.
        /// </summary>
        /// <param name="id">Id of account.</param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [Route("api/person/status")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Status([FromUri] int id, [FromBody] EditStatusViewModel modifier)
        {
            #region Request parameters are invalid

            // Modifier hasn't been initialized.
            if (modifier == null)
            {
                // Initialize modifier and do validation.
                modifier = new EditStatusViewModel();
                Validate(modifier);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result handling

            try
            {
                // Find the person from database using unique identity.
                var person = await _repositoryAccountExtended.FindPersonAsync(id, null, null, null, null);

                // No person has been found.
                if (person == null)
                {
                    // Log the error for future trace.
                    _log.Error($"Person [Id: {id}] doesn't exit in database.");

                    // Tell the client about this error.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Change account status and retrieve the process result.
                person.Status = (byte) modifier.Status;

                // Save changes to database.
                await _repositoryAccountExtended.InitializePersonAsync(person);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Log the error for the future trace.
                _log.Error(exception.Message, exception);

                // Tell the client server has error.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Statistic people role.
        /// </summary>
        /// <param name="summarizer"></param>
        /// <returns></returns>
        [Route("api/person/status/statistic")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Admin})]
        public async Task<HttpResponseMessage> Statistic([FromBody] StatusSummary summarizer)
        {
            // Information hasn't been initialized. Initialize and validate it.
            if (summarizer == null)
            {
                summarizer = new StatusSummary();
                Validate(summarizer);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Find the person from database using unique identity.
            var summaryResult = await _repositoryAccountExtended.SummarizePersonRoleAsync(summarizer.Role);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                ActiveAccounts = summaryResult.Where(x => x.Status == (byte) StatusAccount.Active).Sum(x => x.Total),
                PendingAccounts = summaryResult.Where(x => x.Status == (byte) StatusAccount.Pending).Sum(x => x.Total),
                DeactiveAccounts = summaryResult.Where(x => x.Status == (byte) StatusAccount.Inactive).Sum(x => x.Total),
                Total = summaryResult.Sum(x => x.Total)
            });
        }

        #endregion
    }
}