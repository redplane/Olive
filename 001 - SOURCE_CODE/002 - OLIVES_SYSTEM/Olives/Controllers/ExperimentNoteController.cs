using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.Hubs;
using Olives.Interfaces;
using Olives.Interfaces.Medical;
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
    [Route("api/medical/experiment")]
    public class ExperimentNoteController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryMedicalRecord"></param>
        /// <param name="repositoryExperimentNote"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        /// <param name="notificationService"></param>
        public ExperimentNoteController(IRepositoryMedicalRecord repositoryMedicalRecord,
            IRepositoryExperimentNote repositoryExperimentNote,
            ITimeService timeService, INotificationService notificationService,
            ILog log)
        {
            _repositoryMedicalRecord = repositoryMedicalRecord;
            _repositoryExperimentNote = repositoryExperimentNote;
            _timeService = timeService;
            _notificationService = notificationService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find medical experiment asynchronously by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FindMedicalExperimentAsync([FromUri] int id)
        {
            #region Record filter

            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            var filter = new FilterExperimentNoteViewModel();
            filter.Id = id;
            filter.Requester = requester;

            var result = await _repositoryExperimentNote.FilterExperimentNotesAsync(filter);
            if (result.Total != 1)
            {
                _log.Error($"There is/are {result.Total} medical experiment note [Id:{id}]");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            var experimentNote = result.ExperimentNotes.FirstOrDefault();
            if (experimentNote == null)
            {
                _log.Error($"There is/are {result.Total} medical experiment note [Id:{id}]");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Note = new
                {
                    experimentNote.Id,
                    MedicalRecord = experimentNote.MedicalRecordId,
                    experimentNote.Owner,
                    experimentNote.Name,
                    experimentNote.Info,
                    experimentNote.Time,
                    experimentNote.Created,
                    experimentNote.LastModified
                }
            });
        }

        /// <summary>
        ///     Initialize a medical experiment note with extra information.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializeMedialExperiment(
            [FromBody] InitializeMedicalExperiment initializer)
        {
            #region Parameters validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];


            // Initializer hasn't been initialized.
            if (initializer == null)
            {
                initializer = new InitializeMedicalExperiment();
                Validate(initializer);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Medical record validation

            // Find the medical record first.
            var medicalRecord = await _repositoryMedicalRecord.FindMedicalRecordAsync(initializer.MedicalRecord);

            // Medical record is not found.
            if (medicalRecord == null)
            {
                _log.Error($"Medical record {initializer.MedicalRecord} is not found");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            // No relationship is found
            if (requester.Id != medicalRecord.Owner && requester.Id != medicalRecord.Creator)
            {
                // Log the error first.
                _log.Error(
                    $"Requester [Id: {requester.Id}] is not the creator or owner of medical record [Id: {medicalRecord.Id}]");

                // Tell the client the request is forbidden
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Record initialization

            try
            {
                #region Record initialization

                // Initialize note.
                var note = new ExperimentNote();
                note.Info = JsonConvert.SerializeObject(initializer.Infos);
                note.Time = initializer.Time;
                note.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                note.MedicalRecordId = initializer.MedicalRecord;
                note.Name = initializer.Name;
                note.Owner = medicalRecord.Owner;
                note.Creator = requester.Id;

                note = await _repositoryExperimentNote.InitializeExperimentNote(note);

                #endregion

                #region Notification broadcast

                if (note.Creator != note.Owner)
                {
                    var recipient = note.Owner;
                    if (requester.Id == note.Owner)
                        recipient = note.Creator;

                    var notification = new Notification();
                    notification.Type = (byte) NotificationType.Create;
                    notification.Topic = (byte) NotificationTopic.ExperimentNote;
                    notification.Container = medicalRecord.Id;
                    notification.ContainerType = (byte) NotificationTopic.MedicalRecord;
                    notification.Broadcaster = requester.Id;
                    notification.Recipient = recipient;
                    notification.Record = note.Id;
                    notification.Message = string.Format(Language.NotifyExperimentNoteCreate, requester.FullName);
                    notification.Created = note.Created;

                    // Broadcast the notification with fault tolerant.
                    await _notificationService.BroadcastNotificationAsync(notification, Hub);
                }

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Note = new
                    {
                        note.Id,
                        MedicalRecord = note.MedicalRecordId,
                        note.Owner,
                        note.Name,
                        note.Info,
                        note.Time,
                        note.Created
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }

            #endregion
        }

        /// <summary>
        ///     Initialize a medical experiment note with extra information.
        /// </summary>
        /// <param name="experiment">Experiment which contains records.</param>
        /// <param name="modifier">List of informations which need changing</param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> ModifyMedialExperimentNote([FromUri] int experiment,
            [FromBody] EditMedicalExperiment modifier)
        {
            #region Parameters validation

            // Initializer hasn't been initialized.
            if (modifier == null)
            {
                modifier = new EditMedicalExperiment();
                Validate(modifier);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Record find

            // Find the medical record first.
            var experimentNote = await _repositoryExperimentNote.FindExperimentNoteAsync(experiment);

            if (experimentNote == null)
            {
                // Log error.
                _log.Error($"There is no experiment note [Id: {experiment}]");

                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship find

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester doesn't take part in the medical note.
            if (requester.Id != experimentNote.Creator && requester.Id != experimentNote.Owner)
            {
                _log.Error($"Requester is not the creator of experiment note [Id: {experimentNote.Id}]");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Result handling

            try
            {
                #region Record initialization

                var isDataChanged = false;
                if (modifier.Time != null)
                {
                    experimentNote.Time = modifier.Time.Value;
                    isDataChanged = true;
                }

                // Name is defined
                if (!string.IsNullOrWhiteSpace(modifier.Name))
                {
                    experimentNote.Name = modifier.Name;
                    isDataChanged = true;
                }

                // Information is specified.
                if (modifier.Infos != null)
                {
                    experimentNote.Info = JsonConvert.SerializeObject(modifier.Infos);
                    isDataChanged = true;
                }

                if (isDataChanged)
                {
                    // Update the last modified time.
                    experimentNote.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                    // Update the experiment note.
                    experimentNote = await _repositoryExperimentNote.InitializeExperimentNote(experimentNote);
                }

                #endregion

                #region Notification broadcast

                if (experimentNote.Creator != experimentNote.Owner)
                {
                    var recipient = experimentNote.Owner;
                    if (requester.Id == experimentNote.Owner)
                        recipient = experimentNote.Creator;

                    var notification = new Notification();
                    notification.Type = (byte) NotificationType.Edit;
                    notification.Topic = (byte) NotificationTopic.ExperimentNote;
                    notification.Container = experimentNote.MedicalRecordId;
                    notification.ContainerType = (byte) NotificationTopic.MedicalRecord;
                    notification.Broadcaster = requester.Id;
                    notification.Recipient = recipient;
                    notification.Record = experimentNote.Id;
                    notification.Message = string.Format(Language.NotifyExperimentNoteEdit, requester.FullName);
                    notification.Created = experimentNote.Created;

                    // Broadcast the notification with fault tolerant.
                    await _notificationService.BroadcastNotificationAsync(notification, Hub);
                }

                #endregion

                #region Result handling

                // Send the list of failed record back to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    ExperimentNote = new
                    {
                        experimentNote.Id,
                        MedicalRecord = experimentNote.MedicalRecordId,
                        experimentNote.Name,
                        experimentNote.Info,
                        experimentNote.Time,
                        experimentNote.Created,
                        experimentNote.LastModified
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Delete a medical experiment note or only its key-value pairs.
        /// </summary>
        /// <param name="experiment">Experiment which contains records.</param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeleteMedialExperimentNote([FromUri] int experiment)
        {
            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Filter initialization.
                var filter = new FilterExperimentNoteViewModel();
                filter.Id = experiment;
                filter.Requester = requester;

                // Remove note and retrieve the response.
                var records = await _repositoryExperimentNote.DeleteExperimentNotesAsync(filter);

                // No record has been removed.
                if (records < 1)
                {
                    _log.Error($"There is no experiment note [Id: {experiment}]");

                    // Tell the client that record is not found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Send the list of failed record back to client.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Filter medical by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/experiment/filter")]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterMedicalExperimentNoteAsync(
            [FromBody] FilterExperimentNoteViewModel filter)
        {
            #region Request parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterExperimentNoteViewModel();
                Validate(filter);
            }

            // Request paramters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client.");

                // Tell the client about error.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Medical experiment filter

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Update the filter.
                filter.Requester = requester;

                // Do the filter.
                var result = await _repositoryExperimentNote.FilterExperimentNotesAsync(filter);

                // Tell the client about the filter result.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    ExperimentNotes = result.ExperimentNotes.Select(x => new
                    {
                        x.Id,
                        MedicalRecord = x.MedicalRecordId,
                        x.Owner,
                        x.Creator,
                        x.Name,
                        x.Info,
                        x.Time,
                        x.Created,
                        x.LastModified
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // As the exception happens, log the error.
                _log.Error(exception.Message, exception);

                // Tell the client about the internal server error.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of medical record
        /// </summary>
        private readonly IRepositoryMedicalRecord _repositoryMedicalRecord;

        /// <summary>
        ///     Repository experiment note
        /// </summary>
        private readonly IRepositoryExperimentNote _repositoryExperimentNote;

        /// <summary>
        ///     Notification service which provides functions to access notification broadcast functionalities.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Service which provides functions to access calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}