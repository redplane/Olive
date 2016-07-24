using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
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
        /// <param name="repositoryRelation"></param>
        /// <param name="log"></param>
        public BloodPressureController(IRepositoryAccount repositoryAccount,
            IRepositoryBloodPressure repositoryBloodPressure, IRepositoryRelation repositoryRelation, ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryBloodPressure = repositoryBloodPressure;
            _repositoryRelation = repositoryRelation;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
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
        [OlivesAuthorize(new[] {Role.Patient})]
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
            bloodPressure.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

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
        [OlivesAuthorize(new[] {Role.Patient})]
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
            result.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

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
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Remove the found allergy.
                var deletedRecords = await _repositoryBloodPressure.DeleteBloodPressureNoteAsync(id, requester.Id);
                if (deletedRecords < 1)
                {
                    // Tell front-end, no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
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
            // Model hasn't been initialized.
            if (info == null)
            {
                info = new FilterBloodPressureViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid blood pressure filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Owner has been specified.
            if (info.Owner != null)
            {
                // Owner is the requester.
                if (info.Owner == requester.Id)
                    info.Owner = requester.Id;
                else
                {
                    // Find the relation between the owner and the requester.
                    var relationships = await _repositoryRelation.FindRelationshipAsync(requester.Id, info.Owner.Value,
                        (byte) StatusAccount.Active);

                    // No relationship has been found.
                    if (relationships == null || relationships.Count < 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            Error = $"{Language.WarnHasNoRelationship}"
                        });
                    }
                }
            }
            else
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
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}