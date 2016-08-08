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
    [Route("api/bloodsugar")]
    public class BloodSugarController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositorySugarblood"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        public BloodSugarController(IRepositoryBloodSugar repositorySugarblood, IRepositoryRelation repositoryRelation,
            ILog log,
            ITimeService timeService)
        {
            _repositorySugarblood = repositorySugarblood;
            _repositoryRelation = repositoryRelation;
            _log = log;
            _timeService = timeService;
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
            #region Result find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Retrieve the results list.
            var bloodSugar = await _repositorySugarblood.FindBloodSugarAsync(id);

            // No result has been received.
            if (bloodSugar == null)
            {
                // Log the error.
                _log.Error($"There is no blood sugar [Id: {id}] in database");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            // Find the relationship between requester and the owner.
            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(requester.Id, bloodSugar.Owner);

            // Requester doesn't have relationship with the record owner.
            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error(
                    $"There is no relationship between requester [Id: {requester.Id}] and blood sugar owner [Id: {bloodSugar.Owner}]");

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
                BloodSugar = new
                {
                    bloodSugar.Id,
                    bloodSugar.Owner,
                    bloodSugar.Created,
                    bloodSugar.LastModified,
                    bloodSugar.Note,
                    bloodSugar.Time,
                    bloodSugar.Value
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
        public async Task<HttpResponseMessage> Post([FromBody] InitializeBloodSugarViewModel info)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (info == null)
            {
                info = new InitializeBloodSugarViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid request parameters. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Record initialization & handling

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var bloodSugar = new SugarBlood();
            bloodSugar.Owner = requester.Id;
            bloodSugar.Value = info.Value;
            bloodSugar.Note = info.Note;
            bloodSugar.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            bloodSugar.Time = info.Time;

            // Insert a new allergy to database.
            bloodSugar = await _repositorySugarblood.InitializeSugarbloodNoteAsync(bloodSugar);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodSugar = new
                {
                    bloodSugar.Id,
                    bloodSugar.Created,
                    bloodSugar.Note,
                    bloodSugar.Time,
                    bloodSugar.Value
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
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] EditBloodSugarViewModel modifier)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (modifier == null)
            {
                modifier = new EditBloodSugarViewModel();
                Validate(modifier);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Record find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var bloodSugar = await _repositorySugarblood.FindBloodSugarAsync(id);

            if (bloodSugar == null)
            {
                // Log the error.
                _log.Error($"There is no blood sugar [Id: {id}] in database");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            if (requester.Id != bloodSugar.Owner)
            {
                // Log the error.
                _log.Error($"Requester [Id: {requester.Id}] is not the owner of blood sugar [Id: {bloodSugar.Id}]");

                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Record modification & handling

            if (modifier.Time != null)
                bloodSugar.Time = modifier.Time.Value;

            if (modifier.Value != null)
                bloodSugar.Value = modifier.Value.Value;

            if (!string.IsNullOrWhiteSpace(bloodSugar.Note))
                bloodSugar.Note = modifier.Note;

            bloodSugar.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Update allergy.
            bloodSugar = await _repositorySugarblood.InitializeSugarbloodNoteAsync(bloodSugar);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodSugar = new
                {
                    bloodSugar.Id,
                    bloodSugar.Created,
                    bloodSugar.LastModified,
                    bloodSugar.Note,
                    bloodSugar.Time,
                    bloodSugar.Value
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
            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Filter initialization.
                var filter = new FilterBloodSugarViewModel();
                filter.Id = id;
                filter.Owner = requester.Id;

                // Delete and retrieve the affected records.
                var records = await _repositorySugarblood.DeleteBloodSugarAsync(filter);

                if (records < 1)
                {
                    // Log the error.
                    _log.Error($"There is no blood sugar record [Id: {id}]");

                    // Tell front-end, no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Tell client the result is OK.
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
        [Route("api/bloodsugar/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterBloodSugarViewModel filter)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterBloodSugarViewModel();
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

            // Owner is not specified. That means the requester wants to see his/her records.
            if (filter.Owner == null)
                filter.Owner = requester.Id;

            // Find the relationship between the requester and owner.
            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(filter.Owner.Value, requester.Id);
            if (!isRelationshipAvailable)
            {
                _log.Error(
                    $"There is no relationship between requester [Id: {requester.Id}] and blood sugar owner [Id: {filter.Owner}]");

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    BloodSugars = new object[0],
                    Total = 0
                });
            }

            #endregion

            #region Result handling

            // Retrieve the results list.
            var result = await _repositorySugarblood.FilterBloodSugarAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                BloodSugars = result.Sugarbloods.Select(x => new
                {
                    x.Id,
                    x.Created,
                    x.LastModified,
                    x.Note,
                    x.Owner,
                    x.Time,
                    x.Value
                }),
                result.Total
            });

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of sugarblood notes.
        /// </summary>
        private readonly IRepositoryBloodSugar _repositorySugarblood;

        /// <summary>
        ///     Repository of relationship.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Service which provides function to access
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}