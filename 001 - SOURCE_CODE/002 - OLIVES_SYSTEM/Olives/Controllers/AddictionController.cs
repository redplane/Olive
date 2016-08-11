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
    public class AddictionController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAddiction"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        public AddictionController(IRepositoryAddiction repositoryAddiction,
            IRepositoryRelation repositoryRelation, ITimeService timeService,
            ILog log)
        {
            _repositoryAddiction = repositoryAddiction;
            _repositoryRelation = repositoryRelation;
            _timeService = timeService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find an addiction by using addiction id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/addiction")]
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            #region Record find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find addiction by using id.
            var addiction = await _repositoryAddiction.FindAddictionAsync(id);

            // No result has been received.
            if (addiction == null)
            {
                // Log the error.
                _log.Error($"There is no addiction [Id: {id}] in database");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship find

            // Find the relationship between the requester and the owner.
            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(requester.Id, addiction.Owner);

            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error(
                    $"There is no relationship between requester [Id: {requester.Id}] and the record owner [Id: {addiction.Owner}]");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result handling

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Addiction = new
                {
                    addiction.Id,
                    addiction.Cause,
                    addiction.Created,
                    addiction.LastModified,
                    addiction.Note,
                    addiction.Owner
                }
            });

            #endregion
        }

        /// <summary>
        ///     Add an addiction asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/addiction")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeAddictionViewModel info)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new InitializeAddictionViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Record initialization

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var addiction = new Addiction();
            addiction.Owner = requester.Id;
            addiction.Cause = info.Cause;
            addiction.Note = info.Note;
            addiction.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            #endregion

            #region Result handling

            // Insert a new allergy to database.
            var result = await _repositoryAddiction.InitializeAddictionAsync(addiction);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Addiction = new
                {
                    result.Id,
                    result.Cause,
                    result.Note,
                    result.Created
                }
            });

            #endregion
        }

        /// <summary>
        ///     Edit an addiction asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [Route("api/addiction")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Put([FromUri] int id, EditAddictionViewModel modifier)
        {
            #region Request parameters validation

            // Request parameters haven't been initialized.
            if (modifier == null)
            {
                modifier = new EditAddictionViewModel();
                Validate(modifier);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the errors.
                _log.Error("Request parameters are invalid. Errors sent to client.");

                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Record find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the addiction of the requester with the same id.
            var addiction = await _repositoryAddiction.FindAddictionAsync(id);

            // Invalid record or record is not unique.
            if (addiction == null)
            {
                // Log the error.
                _log.Error($"There is no addiction [Id: {id}] in database");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is not the owner of record.
            if (addiction.Owner != requester.Id)
            {
                // Log the error.
                _log.Error($"Requester is not the owner of addiction [Id: {id}]");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result update & handling

            // Update the information.
            if (!string.IsNullOrEmpty(modifier.Cause))
                addiction.Cause = modifier.Cause;

            if (!string.IsNullOrEmpty(modifier.Note))
                addiction.Note = modifier.Note;

            // Update the last time record was lastly modified.
            addiction.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Update record to database.
            addiction = await _repositoryAddiction.InitializeAddictionAsync(addiction);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Addiction = new
                {
                    addiction.Id,
                    addiction.Cause,
                    addiction.Note,
                    addiction.Created,
                    addiction.LastModified,
                    addiction.Owner
                }
            });

            #endregion
        }

        /// <summary>
        ///     Delete an addiction asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/addiction")]
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Initialize filter.
                var filter = new FilterAddictionViewModel();
                filter.Id = id;
                filter.Owner = requester.Id;

                // Remove the addiction of the requester.
                var affectedRecords = await _repositoryAddiction.DeleteAddictionAsync(filter);

                // No record has been affected.
                if (affectedRecords < 1)
                {
                    // Log the error.
                    _log.Error($"There is no addiction [Id: {id}] with owner [Id: {requester.Id}] in database");

                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Log the exception to file.
                _log.Error(exception.Message, exception);

                // Tell the client that something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        /// <summary>
        ///     Filter list of addictions by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/addiction/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterAddictionViewModel filter)
        {
            #region Request parameter validation

            // Invalid filter.
            if (filter == null)
            {
                filter = new FilterAddictionViewModel();
                Validate(filter);
            }

            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Relationship validation

            // Owner is not specified. That means requester wants to see his/her records.
            if (filter.Owner == null)
                filter.Owner = requester.Id;

            // Check whether there is a relationship between requester and owner or not.
            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(filter.Owner.Value, requester.Id);

            // There is no relationship between 'em.
            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error(
                    $"There is no relationship between requester [Id: {requester.Id}] and record owner [Id: {filter.Owner}]");

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Addictions = new object[0],
                    Total = 0
                });
            }

            #endregion

            #region Result filtering and handling

            // Filter addictions by using specific conditions.
            var result = await _repositoryAddiction.FilterAddictionsAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Addictions = result.Addictions.Select(x => new
                {
                    x.Id,
                    x.Cause,
                    x.Note,
                    x.Created,
                    x.LastModified,
                    x.Owner
                }),
                result.Total
            });

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of allergies
        /// </summary>
        private readonly IRepositoryAddiction _repositoryAddiction;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Service which provides functions for time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}