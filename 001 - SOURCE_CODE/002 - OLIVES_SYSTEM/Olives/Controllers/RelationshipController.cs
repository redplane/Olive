using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Constants;
using Olives.Enumerations;
using Olives.Interfaces;
using Olives.Models;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/relationship")]
    public class RelationshipController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryRelation"></param>
        /// <param name="repositoryStorage"></param>
        /// <param name="timeService"></param>
        /// <param name="applicationSetting"></param>
        /// <param name="log"></param>
        public RelationshipController(
            IRepositoryRelation repositoryRelation,
            IRepositoryStorage repositoryStorage,
            ITimeService timeService,
            ApplicationSetting applicationSetting,
            ILog log)
        {
            _repositoryRelation = repositoryRelation;
            _repositoryStorage = repositoryStorage;

            _timeService = timeService;
            _applicationSetting = applicationSetting;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Request to create a relationship to a target person.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Patient })]
        public async Task<HttpResponseMessage> InitializeRelation([FromBody] InitializeRelationshipViewModel initializer)
        {
            #region Request parameters validation

            // Initializer hasn't been initializer
            if (initializer == null)
            {
                initializer = new InitializeRelationshipViewModel();
                Validate(initializer);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client");

                // Tell the client about this error.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Check whether these two people have relation or not.
                var relationship =
                    await _repositoryRelation.FindRelationshipAsync(requester.Id, initializer.Target, null);

                // 2 people already make a relationship to each other.
                if (relationship != null && relationship.Count > 0)
                {
                    // Relationship has already been registered.
                    _log.Error(
                        $"Relationship from Requester[Id: {requester.Id}] to Owner[Id: {initializer.Target}] exists.");

                    // Tell client about the conflict.
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        Error = $"{Language.WarnRelationshipAlreadyExist}"
                    });
                }

                // Create an instance of relation.
                var relation = new Relation();
                relation.Source = requester.Id;
                relation.Target = initializer.Target;
                relation.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                relation.Status = (byte)StatusRelation.Pending;

                await _repositoryRelation.InitializeRelationAsync(relation);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Exception happens, log the error and tell client about the error.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Confirm a pending relation.
        /// </summary>
        /// <param name="confirmation"></param>
        /// <returns></returns>
        [Route("api/relationship/confirm")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Patient, Role.Doctor })]
        public async Task<HttpResponseMessage> DecideRelationshipAsync(
            [FromBody] ConfirmRelationshipViewModel confirmation)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the relationship by using id.
            var relationship =
                await
                    _repositoryRelation.FindRelationshipAsync(confirmation.Id, requester.Id, RoleRelationship.Target,
                        (byte)StatusRelation.Pending);

            // No relationship has been returned.
            if (relationship == null)
            {
                _log.Error(
                    $"There is no relationship [Id: {confirmation.Id}] targeted to Requester [Id: {requester.Id}]");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRelationNotFound}"
                });
            }

            relationship.Status = (byte)StatusRelation.Active;
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Remove an active relation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] { Role.Patient, Role.Doctor })]
        public async Task<HttpResponseMessage> DeleteRelationship([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Delete relationship and retrieve the number of affected records.
                var records = await _repositoryRelation.DeleteRelationAsync(id, requester.Id, null, null);
                if (records < 1)
                {
                    // No record has been found. Log the error for future trace.
                    _log.Error($"There is no relationship [Id: {id}].");

                    // Tell the client about the rror.
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

                // Tell client there is a problem about this server, please try again.
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        /// <summary>
        ///     Filter relationship by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/relationship/filter/doctor")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Patient })]
        public async Task<HttpResponseMessage> FilterRelatedDoctor([FromBody] FilterRelatedPeopleViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterRelatedPeopleViewModel();
                Validate(filter);
            }

            // Validation is not successful.
            if (!ModelState.IsValid)
            {
                _log.Error("Parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Filter the relationship.
            var result =
                await
                    _repositoryRelation.FilterRelatedDoctorAsync(requester.Id, filter.Status, filter.Page,
                        filter.Records);

            // Find the avatar storage.
            var storageAvatar = _repositoryStorage.FindStorage(Storage.Avatar);

            // Throw the list back to client.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Relationships = result.List.Select(x => new
                {
                    Doctor = new
                    {
                        x.Doctor.Id,
                        x.Doctor.Person.FirstName,
                        x.Doctor.Person.LastName,
                        Specialty = new
                        {
                            x.Doctor.Specialty.Id,
                            x.Doctor.Specialty.Name
                        },
                        x.Doctor.Rank,
                        x.Doctor.Person.Address,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, x.Doctor.Person.Photo,
                                Values.StandardImageExtension),
                        x.Doctor.Person.Phone,
                        x.Doctor.Person.Email
                    },
                    Status = x.RelationshipStatus,
                    x.Created
                }),
                result.Total
            });
        }

        /// <summary>
        ///     Filter relationship by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/relationship/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Patient, Role.Doctor })]
        public async Task<HttpResponseMessage> FilterRelationship([FromBody] FilterRelationshipViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterRelationshipViewModel();
                Validate(filter);
            }

            // Validation is not successful.
            if (!ModelState.IsValid)
            {
                _log.Error("Parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result handling

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Update the filter.
            filter.Requester = requester.Id;

            // Filter the relationship.
            var result =
                await
                    _repositoryRelation.FilterRelationshipAsync(filter);

            // Find the storage of avatar.
            var storageAvatar = _repositoryStorage.FindStorage(Storage.Avatar);

            // Throw the list back to client.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Relationships = result.Relationships.Select(x => new
                {
                    x.Id,
                    Source = new
                    {
                        Id = x.Source,
                        x.Patient.Person.FirstName,
                        x.Patient.Person.LastName,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, x.Patient.Person.Photo, Values.StandardImageExtension)
                    },
                    Target = new
                    {
                        Id = x.Target,
                        x.Doctor.Person.FirstName,
                        x.Doctor.Person.LastName,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, x.Doctor.Person.Photo, Values.StandardImageExtension)
                    },
                    x.Created,
                    x.Status
                }),
                result.Total
            });

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Repository of storage.
        /// </summary>
        private readonly IRepositoryStorage _repositoryStorage;

        /// <summary>
        ///     Instance stores application settings.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

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