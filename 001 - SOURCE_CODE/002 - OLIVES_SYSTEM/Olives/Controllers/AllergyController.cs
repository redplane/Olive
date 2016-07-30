using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
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
    [Route("api/allergy")]
    public class AllergyController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAllergy"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        public AllergyController(IRepositoryAllergy repositoryAllergy,
            IRepositoryRelation repositoryRelation, ITimeService timeService, ILog log)
        {
            _repositoryAllergy = repositoryAllergy;
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

            // Retrieve the results list.
            var allergy = await _repositoryAllergy.FindAllergyAsync(id);

            // No result has been received.
            if (allergy == null)
            {
                // Log the error.
                _log.Error($"There is no allergy [Id: {id}] in database.");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(requester.Id, allergy.Id);
            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error(
                    $"There is no relationship between requester [Id: {requester.Id}] and owner [Id: {allergy.Owner}]");

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
                Allergy = new
                {
                    allergy.Id,
                    allergy.Name,
                    allergy.Cause,
                    allergy.Note,
                    allergy.Created,
                    allergy.LastModified,
                    allergy.Owner
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
        public async Task<HttpResponseMessage> Post([FromBody] InitializeAllergyViewModel info)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize the request view model and do the validation.
                info = new InitializeAllergyViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Record initialization

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var allergy = new Allergy();
            allergy.Owner = requester.Id;
            allergy.Name = info.Name;
            allergy.Cause = info.Cause;
            allergy.Note = info.Note;
            allergy.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert a new allergy to database.
            allergy = await _repositoryAllergy.InitializeAllergyAsync(allergy);

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Allergy = new
                {
                    allergy.Id,
                    allergy.Name,
                    allergy.Cause,
                    allergy.Note,
                    allergy.Created
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
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] EditAllergyViewModel modifier)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (modifier == null)
            {
                modifier = new EditAllergyViewModel();
                Validate(modifier);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var allergy = await _repositoryAllergy.FindAllergyAsync(id);

            // Not record has been found.
            if (allergy == null)
            {
                // Log the error.
                _log.Error($"There is no allergy [Id: {id}] in database");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is not the owner of record.
            if (allergy.Owner != requester.Id)
            {
                // Log the error.
                _log.Error($"Requester [Id: {requester.Id}] is not the owner of allergy [Id: {allergy.Id}]");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result handling

            // Confirm edit.
            if (!string.IsNullOrWhiteSpace(modifier.Name))
                allergy.Name = modifier.Name;

            if (!string.IsNullOrWhiteSpace(modifier.Cause))
                allergy.Cause = modifier.Cause;

            if (!string.IsNullOrWhiteSpace(modifier.Note))
                allergy.Note = modifier.Note;

            // Update time when the record was lastly modified.
            allergy.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Update allergy.
            allergy = await _repositoryAllergy.InitializeAllergyAsync(allergy);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Allergy = new
                {
                    allergy.Id,
                    allergy.Name,
                    allergy.Cause,
                    allergy.Note,
                    allergy.Created,
                    allergy.LastModified
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
                var filter = new FilterAllergyViewModel();
                filter.Id = id;
                filter.Owner = requester.Id;

                // Find and delete the allergy.
                var deletedRecords = await _repositoryAllergy.DeleteAllergyAsync(filter);

                // No record has been deleted.
                if (deletedRecords < 1)
                {
                    // Log the error.
                    _log.Error($"There is no allergy [Id: {id}]");

                    // Tell front-end, no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Tell client the deletion is ok.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Write the error to file.
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
        [Route("api/allergy/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterAllergyViewModel filter)
        {
            #region Request parameters validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Model hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterAllergyViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Relationship validation

            // Owner is not specified. That means the record owner is the requester.
            if (filter.Owner == null)
                filter.Owner = requester.Id;

            // Find the relationship between the owner and requester.
            var isRelationshipAvailable = await _repositoryRelation.IsPeopleConnected(requester.Id, filter.Owner.Value);

            if (!isRelationshipAvailable)
            {
                // Log the error.
                _log.Error(
                    $"There is no relationship between requester [Id: {requester.Id}] and record owner [Id: {filter.Owner}");

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Allergies = new object[0],
                    Total = 0
                });
            }

            #endregion

            #region Result handling

            // Retrieve the results list.
            var result = await _repositoryAllergy.FilterAllergyAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Allergies = result.Allergies
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Cause,
                        x.Note,
                        x.Created,
                        x.LastModified
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
        private readonly IRepositoryAllergy _repositoryAllergy;

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