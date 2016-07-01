using System;
using System.Collections.Generic;
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
using Shared.ViewModels.Response;

namespace Olives.Controllers
{
    [Route("api/bloodpressure")]
    public class BloodPressureController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryBloodPressure"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        public BloodPressureController(IRepositoryAccount repositoryAccount,
            IRepositoryBloodPressure repositoryBloodPressure, ILog log, IEmailService emailService)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryBloodPressure = repositoryBloodPressure;
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
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Retrieve the results list.
            var results = await _repositoryBloodPressure.FindBloodPressureNoteAsync(id, requester.Id);

            // No result has been received.
            if (results == null || results.Count != 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the 1st queried result.
            var result = results
                .Select(x => new BloodPressureViewModel
                {
                    Id = x.Id,
                    Created = x.Created,
                    Diastolic = x.Diastolic,
                    LastModified = x.LastModified,
                    Note = x.Note,
                    Owner = x.Owner,
                    Systolic = x.Systolic,
                    Time = x.Time
                })
                .FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodPressure = result
            });
        }

        /// <summary>
        ///     Insert an allergy asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Patient })]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeBloodPressureViewModel info)
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
            var bloodPressure = new BloodPressure();
            bloodPressure.Diastolic = info.Diastolic;
            bloodPressure.Systolic = info.Systolic;
            bloodPressure.Time = info.Time;
            bloodPressure.Note = info.Note;
            bloodPressure.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryBloodPressure.InitializeBloodPressureNoteAsync(bloodPressure);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodPressure = new
                {
                    result.Id,
                    result.Systolic,
                    result.Diastolic,
                    result.Time,
                    result.Note,
                    result.Created
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
        [OlivesAuthorize(new[] { Role.Patient })]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] InitializeBloodPressureViewModel info)
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
            var results = await _repositoryBloodPressure.FindBloodPressureNoteAsync(id, requester.Id);

            // Not record has been found.
            if (results == null || results.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Records are conflict.
            if (results.Count != 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first record.
            var result = results.FirstOrDefault();
            if (result == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Confirm edit.
            result.Diastolic = info.Diastolic;
            result.Systolic = info.Systolic;
            result.Time = info.Time;
            result.Note = info.Note;
            result.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Update allergy.
            result = await _repositoryBloodPressure.InitializeBloodPressureNoteAsync(result);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodPressure = new BloodPressureViewModel
                {
                    Id = result.Id,
                    Systolic = result.Systolic,
                    Diastolic = result.Diastolic,
                    Time = result.Time,
                    Note = result.Note,
                    Created = result.Created,
                    LastModified = result.LastModified
                }
            });
        }

        /// <summary>
        ///     Delete an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] { Role.Patient })]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var result = await _repositoryBloodPressure.FindBloodPressureNoteAsync(id, requester.Id);

            // Not record has been found.
            if (result == null || result.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Records are conflict.
            if (result.Count != 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first record.
            var heartbeat = result.FirstOrDefault();
            if (heartbeat == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Remove the found allergy.
            _repositoryBloodPressure.DeleteBloodPressureNoteAsync(heartbeat);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/bloodpressure/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterBloodPressureViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                info = new FilterBloodPressureViewModel();
                Validate(info);
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
            var results = await _repositoryBloodPressure.FilterBloodPressureNoteAsync(info);

            // No result has been received.
            if (results == null || results.BloodPressures == null || results.BloodPressures.Count < 1)
            {
                results = new ResponseBloodPressureFilter();
                results.BloodPressures = new List<BloodPressureViewModel>();
                results.Total = 0;

                return Request.CreateResponse(HttpStatusCode.OK, results);
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
        ///     Repository of heartbeats
        /// </summary>
        private readonly IRepositoryBloodPressure _repositoryBloodPressure;

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