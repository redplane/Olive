using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Enumerations;
using Olives.Hubs;
using Olives.Interfaces;
using Olives.Interfaces.Medical;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/medical/note")]
    public class MedicalNoteController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryMedicalNote"></param>
        /// <param name="repositoryMedicalRecord"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="notificationService"></param>
        /// <param name="log"></param>
        public MedicalNoteController(IRepositoryMedicalNote repositoryMedicalNote,
            IRepositoryMedicalRecord repositoryMedicalRecord, IRepositoryRelationship repositoryRelation,
            ITimeService timeService, INotificationService notificationService,
            ILog log)
        {
            _repositoryMedicalNote = repositoryMedicalNote;
            _repositoryMedicalRecord = repositoryMedicalRecord;
            _repositoryRelation = repositoryRelation;
            _timeService = timeService;
            _notificationService = notificationService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FindMedicalNoteAsync([FromUri] int id)
        {
            try
            {
                #region Result filter

                // Retrieve information of person who sent request.
                var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                var filter = new FilterMedicalNoteViewModel();
                filter.Id = id;
                filter.Requester = requester;

                // Do the filter.
                var result = await _repositoryMedicalNote.FilterMedicalNotesAsync(filter);
                if (result.Total != 1)
                {
                    _log.Error($"There is/are {result.Total} medical note [Id: {id}]");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Find the first result.
                var medicalNote = result.MedicalNotes.FirstOrDefault();
                if (medicalNote == null)
                {
                    _log.Error($"There is/are {result.Total} medical note [Id: {id}]");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalNote = new
                    {
                        medicalNote.Id,
                        MedicalRecord = medicalNote.MedicalRecordId,
                        medicalNote.Owner,
                        medicalNote.Creator,
                        medicalNote.Note,
                        medicalNote.Time,
                        medicalNote.Created,
                        medicalNote.LastModified
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Note the exception.
                _log.Error(exception.Message, exception);

                // Tell the client about the terminated process.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializeMedicalNoteAsync(
            [FromBody] InitializeMedicalNoteViewModel initializer)
        {
            #region Paramters validation

            // Model hasn't been initialized.
            if (initializer == null)
            {
                // Initialize it and do the validation.
                initializer = new InitializeMedicalNoteViewModel();
                Validate(initializer);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Medical record validation

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
            
            // Find the medical record.
            var medicalRecord = await _repositoryMedicalRecord.FindMedicalRecordAsync(initializer.MedicalRecord);

            // No medical record has been found.
            if (medicalRecord == null)
            {
                // Log the error and tell client about the result.
                _log.Error($"Medical record [Id: {initializer.MedicalRecord}] is not found");

                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            #endregion

            #region Owner & relationship validation

            // Requester doesn't take part in the medical record.
            if (requester.Id != medicalRecord.Creator && requester.Id != medicalRecord.Owner)
            {
                _log.Error(
                    $"Requester [Id: {requester.Id}] is not the creator of medical record [Id: {medicalRecord.Id}]");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Result handling

            try
            {
                #region Result initialization

                // Initialize an instance of MedicalNote.
                var medicalNote = new MedicalNote();
                medicalNote.MedicalRecordId = initializer.MedicalRecord;
                medicalNote.Creator = requester.Id;
                medicalNote.Owner = medicalRecord.Owner;
                medicalNote.Note = initializer.Note;
                medicalNote.Time = initializer.Time;
                medicalNote.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert a new allergy to database.
                medicalNote = await _repositoryMedicalNote.InitializeMedicalNoteAsync(medicalNote);

                #endregion

                #region Notification broadcast

                // Only the owner should receive the notification.
                if (requester.Id != medicalNote.Owner)
                {
                    var notification = new Notification();
                    notification.Type = (byte) NotificationType.Create;
                    notification.Topic = (byte) NotificationTopic.MedicalNote;
                    notification.Container = medicalRecord.Id;
                    notification.ContainerType = (byte) NotificationTopic.MedicalRecord;
                    notification.Broadcaster = requester.Id;
                    notification.Recipient = medicalNote.Owner;
                    notification.Record = medicalNote.Id;
                    notification.Message = string.Format(Language.NotifyMedicalNoteCreate, requester.FullName);
                    notification.Created = medicalNote.Created;

                    // Broadcast the notification with fault tolerant.
                    await _notificationService.BroadcastNotificationAsync(notification, Hub);
                }

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalNote = new
                    {
                        medicalNote.Id,
                        MedicalRecord = medicalRecord.Id,
                        medicalNote.Owner,
                        medicalNote.Note,
                        medicalNote.Time,
                        medicalNote.Created
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // As the exception happens, log the error first.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> ModifyMedicalNote([FromUri] int id,
            [FromBody] EditMedicalNoteViewModel modifier)
        {
            #region Parameters validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Model hasn't been initialized.
            if (modifier == null)
            {
                // Initialize it and do the validation.
                modifier = new EditMedicalNoteViewModel();
                Validate(modifier);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result find

            // Find the medical note.
            var medicalNote = await _repositoryMedicalNote.FindMedicalNoteAsync(id);

            // Medical note is not found.
            if (medicalNote == null)
            {
                // Log the error and tell client about the result.
                _log.Error($"Medical note [Id: {id}] is not found");

                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship check

            // No relationship is found.
            if (requester.Id != medicalNote.Owner && requester.Id != medicalNote.Creator)
            {
                // Log the error.
                _log.Error(
                    $"Requester [Id: {requester.Id}] is not the creator of medical note [Id: {medicalNote.Id}]");

                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Information update

            try
            {
                #region Result update

                // Note is defined.
                if (modifier.Note != null)
                    medicalNote.Note = modifier.Note;

                // Time is defined.
                if (modifier.Time != null)
                    medicalNote.Time = modifier.Time.Value;

                medicalNote.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert a new allergy to database.
                var result = await _repositoryMedicalNote.InitializeMedicalNoteAsync(medicalNote);

                #endregion

                #region Notification broadcast

                if (medicalNote.Creator != medicalNote.Owner)
                {
                    var recipient = medicalNote.Owner;
                    if (requester.Id == medicalNote.Owner)
                        recipient = medicalNote.Creator;

                    var notification = new Notification();
                    notification.Type = (byte) NotificationType.Create;
                    notification.Topic = (byte) NotificationTopic.MedicalNote;
                    notification.Container = medicalNote.MedicalRecordId;
                    notification.ContainerType = (byte) NotificationTopic.MedicalRecord;
                    notification.Broadcaster = requester.Id;
                    notification.Recipient = recipient;
                    notification.Record = medicalNote.Id;
                    notification.Message = string.Format(Language.NotifyMedicalNoteEdit, requester.FullName);
                    notification.Created = medicalNote.Created;

                    // Broadcast the notification with fault tolerant.
                    await _notificationService.BroadcastNotificationAsync(notification, Hub);
                }

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalNote = new
                    {
                        result.Id,
                        MedicalRecord = result.MedicalRecordId,
                        result.Owner,
                        result.Note,
                        result.Time,
                        result.Created,
                        result.LastModified
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // As the exception happens, log the error first.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Delete a medical note asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeleteMedicalNoteAsync([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Filter initialization.
            var filter = new FilterMedicalNoteViewModel();
            filter.Id = id;
            filter.Requester = requester;
            
            try
            {
                var records = await _repositoryMedicalNote.DeleteMedicalNoteAsync(filter);
                if (records < 1)
                {
                    _log.Error($"There is no medical note [Id: {id}]");
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/note/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterMedicalNote([FromBody] FilterMedicalNoteViewModel filter)
        {
            #region Parameters validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];


            // Model hasn't been initialized.
            if (filter == null)
            {
                // Initialize it and do the validation.
                filter = new FilterMedicalNoteViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Filtering

            try
            {
                // Update the requester.
                filter.Requester = requester;

                // Insert a new allergy to database.
                var result = await _repositoryMedicalNote.FilterMedicalNotesAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalNotes = result.MedicalNotes.Select(x => new
                    {
                        x.Id,
                        MedicalRecord = x.MedicalRecordId,
                        x.Owner,
                        x.Creator,
                        x.Note,
                        x.Time,
                        x.Created,
                        x.LastModified
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // As the exception is thrown, it should be logged.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of medical notes
        /// </summary>
        private readonly IRepositoryMedicalNote _repositoryMedicalNote;

        /// <summary>
        ///     Repository of medical records.
        /// </summary>
        private readonly IRepositoryMedicalRecord _repositoryMedicalRecord;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelationship _repositoryRelation;

        /// <summary>
        ///     Service which provides function to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Service which provides functions to access notification broadcast functions.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}