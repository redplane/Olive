using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Initialize;

namespace Olives.Controllers
{
    [Route("api/")]
    [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
    public class SugarbloodController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositorySugarblood"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        public SugarbloodController(IRepositoryAccount repositoryAccount, IRepositorySugarblood repositorySugarblood,
            ILog log, IEmailService emailService)
        {
            _repositoryAccount = repositoryAccount;
            _repositorySugarblood = repositorySugarblood;
            _log = log;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Retrieve the results list.
            var results = await _repositorySugarblood.FindSugarbloodNoteAsync(id, requester.Id);

            // No result has been received.
            if (results == null || results.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.NoRecordHasBeenFound}
                });
            }

            // Retrieve the 1st queried result.
            var result = results
                .Select(x => new
                {
                    x.Id,
                    x.Value,
                    x.Time,
                    x.Note,
                    x.Created,
                    x.LastModified
                })
                .FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeat = result
            });
        }

        /// <summary>
        ///     Insert an allergy asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeSugarbloodViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] {Language.InvalidRequestParameters}
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var sugarblood = new SugarBlood();
            sugarblood.Owner = requester.Id;
            sugarblood.Value = info.Value;
            sugarblood.Note = info.Note;
            sugarblood.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
            sugarblood.Time = info.Time;

            // Insert a new allergy to database.
            var result = await _repositorySugarblood.InitializeSugarbloodNoteAsync(sugarblood);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Sugarblood = new SugarbloodViewModel
                {
                    Id = result.Id,
                    Created = result.Created,
                    Note = result.Note,
                    Time = result.Time,
                    Value = result.Value
                }
            });
        }

        /// <summary>
        ///     Edit an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] InitializeSugarbloodViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] {Language.InvalidRequestParameters}
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var results = await _repositorySugarblood.FindSugarbloodNoteAsync(id, requester.Id);

            // Not record has been found.
            if (results == null || results.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.WarnRecordNotFound}
                });
            }

            // Records are conflict.
            if (results.Count != 1)
            {
                // Tell front-end that records are conflict.
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Errors = new[] {Language.WarnRecordConflict}
                });
            }

            // Retrieve the first record.
            var result = results.FirstOrDefault();
            if (result == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.WarnRecordNotFound}
                });
            }

            // Confirm edit.
            result.Time = info.Time;
            result.Note = info.Note;
            result.Value = info.Value;
            result.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Update allergy.
            result = await _repositorySugarblood.InitializeSugarbloodNoteAsync(result);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Sugarblood = new SugarbloodViewModel
                {
                    Id = result.Id,
                    Created = result.Created,
                    LastModified = result.LastModified,
                    Note = result.Note,
                    Time = result.Time,
                    Value = result.Value
                }
            });
        }

        /// <summary>
        ///     Delete an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var results = await _repositorySugarblood.FindSugarbloodNoteAsync(id, requester.Id);

            // Not record has been found.
            if (results == null || results.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.WarnRecordNotFound}
                });
            }

            // Records are conflict.
            if (results.Count != 1)
            {
                // Tell front-end that records are conflict.
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Errors = new[] {Language.WarnRecordConflict}
                });
            }

            // Retrieve the first record.
            var result = results.FirstOrDefault();
            if (result == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.WarnRecordNotFound}
                });
            }

            // Remove the found allergy.
            _repositorySugarblood.DeleteSugarbloodNoteAsync(result);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/heartbeat/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterSugarbloodViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] {Language.InvalidRequestParameters}
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Person can only see his/her notes.
            info.Owner = requester.Id;

            // Retrieve the results list.
            var results = await _repositorySugarblood.FilterSugarbloodNoteAsync(info);

            // No result has been received.
            if (results == null || results.Sugarbloods == null || results.Sugarbloods.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.NoRecordHasBeenFound}
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of sugarblood notes.
        /// </summary>
        private readonly IRepositorySugarblood _repositorySugarblood;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used for sending emails.
        /// </summary>
        private readonly IEmailService _emailService;

        #endregion
    }
}