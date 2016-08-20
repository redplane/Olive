using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Interfaces.PersonalNote;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Edit.Personal;
using Olives.ViewModels.Filter.Personal;
using Olives.ViewModels.Initialize;
using Olives.ViewModels.Initialize.Personal;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/bloodpressure")]
    public class BloodPressureController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryBloodPressure"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        public BloodPressureController(IRepositoryBloodPressure repositoryBloodPressure,
            IRepositoryRelationship repositoryRelation, ITimeService timeService, ILog log)
        {
            _repositoryBloodPressure = repositoryBloodPressure;
            _repositoryRelation = repositoryRelation;
            _timeService = timeService;
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
            #region Record find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Retrieve the result.
            var bloodPressureNote = await _repositoryBloodPressure.FindBloodPressureAsync(id);

            // No result has been received.
            if (bloodPressureNote == null)
            {
                // Log the error.
                _log.Error($"There is no blood pressure [Id: {id}] found in database");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship check

            // Check the relationship between the requester and owner.
            var isRelationshipAvailable =
                await _repositoryRelation.IsPeopleConnected(requester.Id, bloodPressureNote.Owner);

            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error($"There is no blood pressure [Id: {id}] found in database");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result handling

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodPressure = new
                {
                    bloodPressureNote.Id,
                    bloodPressureNote.Created,
                    bloodPressureNote.Diastolic,
                    bloodPressureNote.LastModified,
                    bloodPressureNote.Note,
                    bloodPressureNote.Owner,
                    bloodPressureNote.Systolic,
                    bloodPressureNote.Time
                }
            });

            #endregion
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
            #region Request parameter validation

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

            #region Record initalization

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var bloodPressure = new BloodPressure();
            bloodPressure.Owner = requester.Id;
            bloodPressure.Diastolic = info.Diastolic;
            bloodPressure.Systolic = info.Systolic;
            bloodPressure.Time = info.Time;
            bloodPressure.Note = info.Note;
            bloodPressure.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            #endregion

            #region Result handling

            // Insert a new allergy to database.
            bloodPressure = await _repositoryBloodPressure.InitializeBloodPressureAsync(bloodPressure);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodPressure = new
                {
                    bloodPressure.Id,
                    bloodPressure.Systolic,
                    bloodPressure.Diastolic,
                    bloodPressure.Time,
                    bloodPressure.Note,
                    bloodPressure.Created
                }
            });

            #endregion
        }

        /// <summary>
        ///     Edit an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] EditBloodPressureViewModel modifier)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (modifier == null)
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

            #region Result find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var bloodPressureNote = await _repositoryBloodPressure.FindBloodPressureAsync(id);

            // Not record has been found.
            if (bloodPressureNote == null)
            {
                // Log the error.
                _log.Error($"Blood pressure note [Id: {id}] is not found");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is not the owner of record.
            if (bloodPressureNote.Owner != requester.Id)
            {
                // Log the error.
                _log.Error(
                    $"Requester [Id: {requester.Id}] is not the owner of blood pressure [Id: {bloodPressureNote.Id}]");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result handling

            if (modifier.Diastolic != null)
                bloodPressureNote.Diastolic = modifier.Diastolic.Value;

            if (modifier.Systolic != null)
                bloodPressureNote.Systolic = modifier.Systolic.Value;

            if (modifier.Time != null)
                bloodPressureNote.Time = modifier.Time.Value;

            if (!string.IsNullOrEmpty(modifier.Note))
                bloodPressureNote.Note = modifier.Note;

            // Update the last modified time.
            bloodPressureNote.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Update the record.
            bloodPressureNote = await _repositoryBloodPressure.InitializeBloodPressureAsync(bloodPressureNote);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodPressure = new
                {
                    bloodPressureNote.Id,
                    bloodPressureNote.Systolic,
                    bloodPressureNote.Diastolic,
                    bloodPressureNote.Time,
                    bloodPressureNote.Note,
                    bloodPressureNote.Created,
                    bloodPressureNote.LastModified
                }
            });

            #endregion
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
                // Filter initialization.
                var filter = new FilterBloodPressureViewModel();
                filter.Id = id;
                filter.Owner = requester.Id;

                // Remove the found allergy.
                var deletedRecords = await _repositoryBloodPressure.DeleteBloodPressureAsync(filter);
                if (deletedRecords < 1)
                {
                    // Log the error.
                    _log.Error($"There is no blood pressure note [Id: {id} in database");

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
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/bloodpressure/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterBloodPressureViewModel filter)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterBloodPressureViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid blood pressure filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Relationship validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Owner hasn't been specified. That means the records belong to the requester.
            if (filter.Owner == null)
                filter.Owner = requester.Id;

            // Check the relationship between the requester and owner.
            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(requester.Id, filter.Owner.Value);
            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error(
                    $"There is no relationship between requester [Id: {requester.Id}] and record owner [Id: {filter.Owner}]");

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    BloodPressures = new object[0],
                    Total = 0
                });
            }

            #endregion

            #region Result handling

            // Retrieve the results list.
            var result = await _repositoryBloodPressure.FilterBloodPressureAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodPressures = result.BloodPressures.Select(x => new
                {
                    x.Id,
                    x.Created,
                    x.Diastolic,
                    x.LastModified,
                    x.Note,
                    x.Owner,
                    x.Systolic,
                    x.Time
                }),
                result.Total
            });

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of heartbeats
        /// </summary>
        private readonly IRepositoryBloodPressure _repositoryBloodPressure;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelationship _repositoryRelation;

        /// <summary>
        ///     Time service which provides functions to access time calculation functions.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}