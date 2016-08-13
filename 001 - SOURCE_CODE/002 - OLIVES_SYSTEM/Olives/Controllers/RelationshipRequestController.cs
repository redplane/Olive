using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Constants;
using Olives.Hubs;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Olives.Controllers
{
    [Route("api/relationship/request")]
    public class RelationshipRequestController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryRelation"></param>
        /// <param name="repositoryRelationshipRequest"></param>
        /// <param name="repositoryStorage"></param>
        /// <param name="timeService"></param>
        /// <param name="notificationService"></param>
        /// <param name="log"></param>
        public RelationshipRequestController(
            IRepositoryRelationship repositoryRelation,
            IRepositoryRelationshipRequest repositoryRelationshipRequest,
            IRepositoryStorage repositoryStorage,
            ITimeService timeService,
            INotificationService notificationService,
            ILog log)
        {
            _repositoryRelation = repositoryRelation;
            _repositoryRelationshipRequest = repositoryRelationshipRequest;
            _repositoryStorage = repositoryStorage;
            _timeService = timeService;
            _notificationService = notificationService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find the relationship request by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] { Role.Patient, Role.Doctor })]
        public async Task<HttpResponseMessage> FindRelationshipRequest([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
            
            // Filter initialization.
            var filter = new FilterRelationshipRequestViewModel();
            filter.Id = id;
            filter.Requester = requester;
            
            // Find the relationship.
            var relationshipRequest = await _repositoryRelationshipRequest.FindRelationshipRequest(filter);

            // Request is not found.
            if (relationshipRequest == null)
            {
                _log.Error($"There is no relationship [Id: {id}] is found");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }
            
            // Find the storage where avatars are located.
            var storageAvatar = _repositoryStorage.FindStorage(Storage.Avatar);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                RelationshipRequest = new
                {
                    relationshipRequest.Id,
                    Source = new
                    {
                        Id = relationshipRequest.Source,
                        relationshipRequest.Patient.Person.FirstName,
                        relationshipRequest.Patient.Person.LastName,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, relationshipRequest.Patient.Person.Photo,
                                Values.StandardImageExtension)
                    },
                    Target = new
                    {
                        Id = relationshipRequest.Target,
                        relationshipRequest.Doctor.Person.FirstName,
                        relationshipRequest.Doctor.Person.LastName,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, relationshipRequest.Doctor.Person.Photo,
                                Values.StandardImageExtension)
                    },
                    relationshipRequest.Created
                }
            });
        }

        /// <summary>
        ///     Request to create a relationship to a target person.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> InitializeRelationshipRequest(
            [FromBody] InitializeRelationshipRequestViewModel initializer)
        {
            #region Request parameters validation

            // Initializer hasn't been initializer
            if (initializer == null)
            {
                initializer = new InitializeRelationshipRequestViewModel();
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
                #region Relationship validation

                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Requester and target are already connected.
                var rPeopleConnected = await _repositoryRelation.IsPeopleConnected(requester.Id, initializer.Target);

                // 2 people already make a relationship to each other.
                if (rPeopleConnected)
                {
                    _log.Error(
                        $"Relationship from Requester[Id: {requester.Id}] to Owner[Id: {initializer.Target}] exists.");
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        Error = $"{Language.WarnRelationshipAlreadyExist}"
                    });
                }

                #endregion

                #region Relationship request validation

                var filter = new FilterRelationshipRequestViewModel();
                filter.Requester = requester;
                filter.Partner = initializer.Target;

                // Find the relationship request.
                var relationshipRequest = await _repositoryRelationshipRequest.FindRelationshipRequest(filter);

                if (relationshipRequest != null)
                {
                    _log.Error($"Relationship is already in the system");
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        Error = $"{Language.WarnRecordConflict}"
                    });
                }

                #endregion

                #region Initialize relationship request

                // Calculate the current unix time.
                var unix = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Initialize relationship request.
                relationshipRequest = new RelationshipRequest();
                relationshipRequest.Source = requester.Id;
                relationshipRequest.Target = initializer.Target;
                relationshipRequest.Content = initializer.Content;
                relationshipRequest.Created = unix;

                relationshipRequest = await _repositoryRelationshipRequest.InitializeRelationshipRequest(relationshipRequest);

                #endregion

                #region Broadcast notification
                
                var notification = new Notification();
                notification.Type = (byte)NotificationType.Create;
                notification.Topic = (byte)NotificationTopic.RelationshipRequest;
                notification.Container = relationshipRequest.Id;
                notification.ContainerType = (byte)NotificationTopic.RelationshipRequest;
                notification.Broadcaster = requester.Id;
                notification.Recipient = initializer.Target;
                notification.Record = relationshipRequest.Id;
                notification.Message = string.Format(Language.NotifyRelationshipRequestCreate, requester.FullName);
                notification.Created = unix;

                // Broadcast the notification with fault tolerant.
                await _notificationService.BroadcastNotificationAsync(notification, Hub);

                #endregion

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
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/relationship/request/confirm")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> ConfirmRelationshipRequest([FromUri] int id)
        {
            
            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
                
                // Retrieve the current time.
                var unix = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                #region Find the relationship request

                var filter = new FilterRelationshipRequestViewModel();
                filter.Id = id;
                filter.Requester = requester;
                var relationshipRequest = await _repositoryRelationshipRequest.FindRelationshipRequest(filter);

                // No relationship request is found.
                if (relationshipRequest == null)
                {
                    _log.Error($"Relationship request [Id: {id}] is not found in system");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                #endregion
                
                #region Broadcast notification

                var notification = new Notification();
                notification.Type = (byte)NotificationType.Confirm;
                notification.Topic = (byte)NotificationTopic.RelationshipRequest;
                notification.Container = relationshipRequest.Id;
                notification.ContainerType = (byte)NotificationTopic.RelationshipRequest;
                notification.Broadcaster = requester.Id;
                notification.Recipient = relationshipRequest.Source;
                notification.Record = relationshipRequest.Id;
                notification.Message = string.Format(Language.NotifyRelationshipRequestConfirm, requester.FullName);
                notification.Created = unix;

                // Broadcast the notification with fault tolerant.
                await _notificationService.BroadcastNotificationAsync(notification, Hub);

                #endregion

                // Delete relationship and retrieve the number of affected records.
                await _repositoryRelationshipRequest.InitializeRelationship(relationshipRequest);

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
        ///     Delete the relationship request by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> DeleteRelationshipRequest([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                var filter = new FilterRelationshipRequestViewModel();
                filter.Id = id;
                var relationshipRequest = await _repositoryRelationshipRequest.FindRelationshipRequest(filter);

                // No relationship request is found.
                if (relationshipRequest == null || requester.Id != relationshipRequest.Source ||
                    requester.Id != relationshipRequest.Target)
                {
                    _log.Error($"Relationship [Id: {id}] is not found in system");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Delete relationship and retrieve the number of affected records.
                var records = await _repositoryRelationshipRequest.DeleteRelationshipRequest(filter);

                // No record has been deleted.
                if (records < 1)
                {
                    // No record has been found. Log the error for future trace.
                    _log.Error($"There is no relationship request [Id: {id}].");

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
        [Route("api/relationship/request/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> FilterRelationshipRequests(
            [FromBody] FilterRelationshipRequestViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterRelationshipRequestViewModel();
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
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Update the filter.
            filter.Requester = requester;

            // Filter the relationship.
            var result =
                await
                    _repositoryRelationshipRequest.FilterRelationshipRequest(filter);

            // Find the storage of avatar.
            var storageAvatar = _repositoryStorage.FindStorage(Storage.Avatar);

            // Throw the list back to client.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                RelationshipRequests = result.RelationshipRequests.Select(x => new
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
                    x.Created
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
        private readonly IRepositoryRelationship _repositoryRelation;

        /// <summary>
        ///     Repository of relationship.
        /// </summary>
        private readonly IRepositoryRelationshipRequest _repositoryRelationshipRequest;

        /// <summary>
        ///     Repository of storage.
        /// </summary>
        private readonly IRepositoryStorage _repositoryStorage;

        /// <summary>
        ///     Service which provides functions to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        /// Service which provides functions to access notification broadcast.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}