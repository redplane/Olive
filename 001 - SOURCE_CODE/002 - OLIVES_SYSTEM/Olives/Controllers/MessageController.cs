using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Hubs;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/message")]
    public class MessageController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryMessage"></param>
        /// <param name="repositoryRealTimeConnection"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        public MessageController(IRepositoryMessage repositoryMessage,
            IRepositoryRealTimeConnection repositoryRealTimeConnection, IRepositoryRelation repositoryRelation,
            ILog log, ITimeService timeService)
        {
            _repositoryMessage = repositoryMessage;
            _repositoryRealTimeConnection = repositoryRealTimeConnection;
            _repositoryRelation = repositoryRelation;
            _log = log;
            _timeService = timeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find a message by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> FindMessageAsync([FromUri] int id)
        {
            // Find the message.
            var message = await _repositoryMessage.FindMessageAsync(id);

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // The requester didn't take part in the message broadcasting.
            if (!(message.Broadcaster == requester.Id || message.Recipient == requester.Id))
            {
                _log.Error($"Requester [Id: {requester.Id}] is not the broadcaster or recipient");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Return the message to requester.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Message = new
                {
                    message.Id,
                    message.Broadcaster,
                    message.Recipient,
                    message.Content,
                    message.Created,
                    message.IsSeen
                }
            });
        }

        /// <summary>
        /// Initialize a message from broadcaster and recipient.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> InitializeMessageAsync([FromBody] InitializeMessageViewModel initializer)
        {
            #region Request parameter validation

            if (initializer == null)
            {
                initializer = new InitializeMessageViewModel();
                Validate(initializer);
            }

            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Relationship check

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // User cannot send message to recipient.
            if (requester.Id == initializer.Recipient)
            {
                _log.Error($"Requester [Id: {requester.Id}] cannot send message to him/herself");
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error = $"{Language.WarnRecordConflict}"
                });
            }

            // Users dont have relationships with each other.
            var isRelationshipAvailable =
                await _repositoryRelation.IsPeopleConnected(requester.Id, initializer.Recipient);

            if (!isRelationshipAvailable)
            {
                _log.Error($"There is no relationship between requester [Id: {requester.Id}] and recipient [Id: {initializer.Recipient}]");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
                });
            }

            #endregion

            #region Record initialization & handling
            
            var message = new Message();
            message.Broadcaster = requester.Id;
            message.Recipient = initializer.Recipient;
            message.Content = initializer.Content;
            message.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            message.IsSeen = false;

            try
            {
                await _repositoryMessage.BroadcastMessageAsync(message);
                
                #region Message notification

                // As the notification is created successfully. Notification should be sent.
                var connectionIndexes =
                    await
                        _repositoryRealTimeConnection.FindRealTimeConnectionIndexesAsync(initializer.Recipient, null, null);

                // Send notification to all connection indexes which have been found.
                Hub.Clients.Clients(connectionIndexes)
                    .notifyMessage(new 
                    {
                        broadcaster = requester.Id,
                        recipient = initializer.Recipient,
                        content = initializer.Content,
                        created = message.Created
                    });

                #endregion
                
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Message = new
                    {
                        message.Id,
                        message.Broadcaster,
                        message.Recipient,
                        message.Content,
                        message.Created,
                        message.IsSeen
                    }
                });
            }
            catch (Exception exception)
            {
                // Error happens, log it and tell the client.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }

            #endregion
        }

        /// <summary>
        /// Filter messages list by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/message/filter")]
        public async Task<HttpResponseMessage> FilterMessagesAsync([FromBody] FilterMessageViewModel filter)
        {
            #region Request parameters validation

            if (filter == null)
            {
                filter = new FilterMessageViewModel();
                Validate(filter);
            }

            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Filter & result handling
            
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Update the information of filter.
            filter.Requester = requester.Id;
            
            // Do filter.
            var result = await _repositoryMessage.FilterMessagesAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Messages = result.Messages.Select(x => new
                {
                    x.Id,
                    x.Broadcaster,
                    x.Recipient,
                    x.Content,
                    x.Created,
                    x.IsSeen
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
        ///     Repository which provides function to access real time connection database.
        /// </summary>
        private readonly IRepositoryRealTimeConnection _repositoryRealTimeConnection;

        /// <summary>
        /// Repository which provides function to access message database.
        /// </summary>
        private readonly IRepositoryMessage _repositoryMessage;

        /// <summary>
        /// Service which provides functions to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}