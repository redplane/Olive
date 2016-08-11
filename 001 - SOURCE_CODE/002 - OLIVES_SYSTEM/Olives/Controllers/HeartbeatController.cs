using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces.PersonalNote;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/heartbeat")]
    [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
    public class HeartbeatController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryHeartbeat"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        public HeartbeatController(IRepositoryHeartbeat repositoryHeartbeat,
            IRepositoryRelation repositoryRelation, ITimeService timeService,
            ILog log)
        {
            _repositoryHeartbeat = repositoryHeartbeat;
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
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Retrieve the results list.
            var heartbeat = await _repositoryHeartbeat.FindHeartbeatAsync(id);

            // No result has been received.
            if (heartbeat == null)
            {
                // Log the error.
                _log.Error($"Heartbeat [Id: {id}] doesn't exist in database");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Check whether the request has connection with the owner or not.
            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(requester.Id, heartbeat.Owner);
            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error(
                    $"There is no relationship between Requester[Id: {requester.Id}] and heart beat owner [Id: {heartbeat.Owner}]");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeat = new
                {
                    heartbeat.Id,
                    heartbeat.Created,
                    heartbeat.LastModified,
                    heartbeat.Time,
                    heartbeat.Note,
                    heartbeat.Rate
                }
            });
        }

        /// <summary>
        ///     Insert an allergy asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeHeartbeatViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                info = new InitializeHeartbeatViewModel();
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

            // Only filter and receive the first result.
            var heartbeat = new Heartbeat();
            heartbeat.Owner = requester.Id;
            heartbeat.Rate = info.Rate;
            heartbeat.Note = info.Note;
            heartbeat.Time = info.Time;
            heartbeat.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert a new allergy to database.
            var result = await _repositoryHeartbeat.InitializeHeartbeatNoteAsync(heartbeat);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeat = new
                {
                    result.Id,
                    result.Rate,
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
        /// <param name="modifier"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] EditHeartbeatViewModel modifier)
        {
            #region Request parameters are invalid

            // Model hasn't been initialized.
            if (modifier == null)
            {
                modifier = new EditHeartbeatViewModel();
                Validate(modifier);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Record validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var heartbeat = await _repositoryHeartbeat.FindHeartbeatAsync(id);

            // Not record has been found.
            if (heartbeat == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is not the owner of record.
            if (heartbeat.Owner != requester.Id)
            {
                // Log the error.
                _log.Error($"Requester [Id: {requester.Id}] is not the owner of heartbeat [Id: {heartbeat.Id}]");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result handling

            if (modifier.Rate != null)
                heartbeat.Rate = modifier.Rate.Value;

            if (modifier.Time != null)
                heartbeat.Time = modifier.Time.Value;

            if (!string.IsNullOrEmpty(modifier.Note))
                heartbeat.Note = modifier.Note;

            heartbeat.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Update heartbeat.
            heartbeat = await _repositoryHeartbeat.InitializeHeartbeatNoteAsync(heartbeat);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeat = new
                {
                    heartbeat.Id,
                    heartbeat.Time,
                    heartbeat.Created,
                    heartbeat.LastModified,
                    heartbeat.Note,
                    heartbeat.Rate
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
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Filter initialization.
                var filter = new FilterHeatbeatViewModel();
                filter.Id = id;
                filter.Owner = requester.Id;

                // Remove the found allergy.
                var deletedRecords = await _repositoryHeartbeat.DeleteHeartbeatNoteAsync(filter);

                // No record has been deleted.
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
                // Log the error to file.
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
        [Route("api/heartbeat/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterHeatbeatViewModel filter)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterHeatbeatViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Relationship validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // No owner is specified. That means the owner is the requester.
            if (filter.Owner == null)
                filter.Owner = requester.Id;
            else
            {
                // Check the relationship between the owner and requester.
                var isRelationshipAvailable =
                    await _repositoryRelation.IsPeopleConnected(requester.Id, filter.Owner.Value);

                // There is no active relationship between 'em.
                if (!isRelationshipAvailable)
                {
                    // Log the error.
                    _log.Error(
                        $"There is no relationship between requester [Id: {requester.Id}] and owner [Id:{filter.Owner}]");

                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Heartbeats = new object[0],
                        Total = 0
                    });
                }
            }

            #endregion

            #region Result handling

            // Retrieve the results list.
            var result = await _repositoryHeartbeat.FilterHeartbeatAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeats = result.Heartbeats.Select(x => new
                {
                    x.Id,
                    x.Created,
                    x.LastModified,
                    x.Note,
                    x.Owner,
                    x.Rate,
                    x.Time
                })
            });

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of heartbeats
        /// </summary>
        private readonly IRepositoryHeartbeat _repositoryHeartbeat;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Service which provides functions to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}