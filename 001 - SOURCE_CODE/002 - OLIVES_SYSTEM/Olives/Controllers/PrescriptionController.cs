﻿using System;
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
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;
using Olives.Controllers;

namespace Olives.Controllers
{
    [Route("api/medical/prescription")]
    public class PrescriptionController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="repositoryMedicalRecord"></param>
        /// <param name="repositoryPrescription"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="notificationService"></param>
        /// <param name="log"></param>
        public PrescriptionController(IRepositoryAccountExtended repositoryAccountExtended,
            IRepositoryMedicalRecord repositoryMedicalRecord, IRepositoryPrescription repositoryPrescription,
            IRepositoryRelation repositoryRelation,
            ITimeService timeService, INotificationService notificationService,
            ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositoryMedicalRecord = repositoryMedicalRecord;
            _repositoryPrescription = repositoryPrescription;
            _repositoryRelation = repositoryRelation;
            _notificationService = notificationService;
            _timeService = timeService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Retrieve a prescription by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FindPrescriptionAsync([FromUri] int id)
        {
            #region Prescription validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the prescription by using id.
            var prescription = await _repositoryPrescription.FindPrescriptionAsync(id);

            // No record is found.
            if (prescription == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Owner validation

            // Find the owner of medical record.
            var owner =
                await _repositoryAccountExtended.FindPersonAsync(prescription.Owner, null, null, null, StatusAccount.Active);
            if (owner == null)
            {
                // Tell requester the record isn't found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            // Requester is different from the medical record owner.
            if (requester.Id != owner.Id)
            {
                // Only prescription created by patient can be viewed by doctor who has relationship with owner..
                if (requester.Id != prescription.Creator && prescription.Creator != prescription.Owner)
                {
                    // Log the error.
                    _log.Error($"Requester [Id: [{requester.Id}] is not the creator of Prescription [{prescription.Id}]");

                    // Tell the client that the record is not found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Find the relationship between requester and owner.
                var relationships = await _repositoryRelation.FindRelationshipAsync(requester.Id, owner.Id,
                    (byte) StatusRelation.Active);

                // No active relationship is found.
                if (relationships == null || relationships.Count < 1)
                {
                    // Tell requester the record isn't found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }
            }

            #endregion

            #region Response initialization

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Prescription = new
                {
                    prescription.Id,
                    MedicalRecord = prescription.MedicalRecordId,
                    prescription.From,
                    prescription.To,
                    prescription.Medicine,
                    prescription.Note,
                    prescription.Created,
                    prescription.LastModified
                }
            });

            #endregion
        }

        /// <summary>
        ///     Initialize a prescription.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializePrescriptionAsync(
            [FromBody] InitializePrescriptionViewModel info)
        {
            #region Parameters validation

            // Information hasn't been initialized.
            if (info == null)
            {
                info = new InitializePrescriptionViewModel();
                Validate(info);
            }

            // Invalid submitted parameters.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameter is invalid. Error sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Medical record validation

            // Find the medical record first.
            var medicalRecord = await _repositoryMedicalRecord.FindMedicalRecordAsync(info.MedicalRecord);
            if (medicalRecord == null)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            #endregion

            #region Medical record owner validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            if (requester.Id != medicalRecord.Creator && requester.Id != medicalRecord.Owner)
            {
                _log.Error($"Requester [Id: {requester.Id}] is not the creator or owner of medical record [Id: {medicalRecord.Id}]");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Information update

            try
            {
                #region Prescription initialization

                var prescription = new Prescription();
                prescription.Owner = medicalRecord.Owner;
                prescription.MedicalRecordId = medicalRecord.Id;
                prescription.From = info.From;
                prescription.To = info.To;
                if (info.Medicines != null)
                    prescription.Medicine = JsonConvert.SerializeObject(info.Medicines);

                prescription.Note = info.Note;
                prescription.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Initialize prescription to database.
                prescription = await _repositoryPrescription.InitializePrescriptionAsync(prescription);

                #endregion

                #region Notification broadcast

                var recipient = prescription.Owner;
                if (requester.Id == prescription.Owner)
                    recipient = prescription.Creator;

                var notification = new Notification();
                notification.Type = (byte)NotificationType.Create;
                notification.Topic = (byte)NotificationTopic.Prescription;
                notification.Broadcaster = requester.Id;
                notification.Recipient = recipient;
                notification.Record = prescription.Id;
                notification.Message = string.Format(Language.NotifyPrescriptionCreate, requester.FullName);
                notification.Created = prescription.Created;

                // Broadcast the notification with fault tolerant.
                await _notificationService.BroadcastNotificationAsync(notification, Hub);

                #endregion

                #region Result return

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    prescription.Id,
                    MedicalRecord = prescription.MedicalRecordId,
                    prescription.From,
                    prescription.To,
                    prescription.Name,
                    prescription.Medicine,
                    prescription.Note,
                    prescription.Created
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                // Tell the client about the internal error of server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Edit a prescription.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> EditPrescriptionAsync([FromUri] int id,
            [FromBody] EditPrescriptionViewModel info)
        {
            #region Parameters validation

            // Information hasn't been initialized.
            if (info == null)
            {
                info = new EditPrescriptionViewModel();
                Validate(info);
            }

            // Invalid submitted parameters.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameter is invalid. Error sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Prescription validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];


            // Find the prescription by using id.
            var prescription = await _repositoryPrescription.FindPrescriptionAsync(id);
            if (prescription == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Prescription owner validation

            // Requester is not the creator or owner of prescription.
            if (requester.Id != prescription.Creator && requester.Id != prescription.Owner)
            {
                _log.Error($"Requester [Id: {requester.Id}] is not prescription [Id:{prescription.Id}] owner or creator");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Information construction

            try
            {
                #region Information update

                if (info.From != null)
                    prescription.From = info.From.Value;

                if (info.To != null)
                    prescription.To = info.To.Value;

                if (info.Medicines != null)
                    prescription.Medicine = JsonConvert.SerializeObject(info.Medicines);

                if (!string.IsNullOrEmpty(info.Note))
                    prescription.Note = info.Note;

                // Update last modified time.
                var unix = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                prescription.LastModified = unix;

                // Initialize prescription to database.
                prescription = await _repositoryPrescription.InitializePrescriptionAsync(prescription);

                #endregion
                
                #region Notification broadcast

                var recipient = prescription.Owner;
                if (requester.Id == prescription.Owner)
                    recipient = prescription.Creator;
                
                var notification = new Notification();
                notification.Type = (byte)NotificationType.Edit;
                notification.Topic = (byte)NotificationTopic.Prescription;
                notification.Broadcaster = requester.Id;
                notification.Recipient = recipient;
                notification.Record = prescription.Id;
                notification.Message = string.Format(Language.NotifyPrescriptionModified, requester.FullName);
                notification.Created = unix;

                // Broadcast the notification with fault tolerant.
                await _notificationService.BroadcastNotificationAsync(notification, Hub);
                
                #endregion
                
                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    prescription.Id,
                    MedicalRecord = prescription.MedicalRecordId,
                    prescription.From,
                    prescription.To,
                    prescription.Name,
                    prescription.Medicine,
                    prescription.Note,
                    prescription.Created,
                    prescription.LastModified
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Log the excepion.
                _log.Error(exception.Message, exception);

                // Tell the client server is facing with an error.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Edit a prescription.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeletePrescriptionAsync([FromUri] int id)
        {
            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Patient can only delete his/her record.
                var records = await _repositoryPrescription.DeletePrescriptionAsync(id, requester.Id);

                // No record has been deleted.
                if (records < 1)
                {
                    _log.Error($"There is no prescription [Id: {id}] whose owner is [Id: {requester.Id}]");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }
                
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Log the error first.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Edit a prescription.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/prescription/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterPrescriptionAsync([FromBody] FilterPrescriptionViewModel filter)
        {
            #region Parameters validation

            // Information hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterPrescriptionViewModel();
                Validate(filter);
            }

            // Invalid submitted parameters.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameter is invalid. Error sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Filter initialization

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
                filter.Requester = requester.Id;


                // Filter prescription by using specific conditions.
                var result = await _repositoryPrescription.FilterPrescriptionAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Prescriptions = result.Prescriptions.Select(x => new
                    {
                        x.Id,
                        MedicalRecord = x.MedicalRecordId,
                        x.Owner,
                        x.From,
                        x.To,
                        x.Name,
                        x.Medicine,
                        x.Note,
                        x.Created,
                        x.LastModified
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // As the exception happens, log it first.
                _log.Error(exception.Message, exception);

                // Tell the client about the terminated process.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Repository of medical record
        /// </summary>
        private readonly IRepositoryMedicalRecord _repositoryMedicalRecord;

        /// <summary>
        ///     Repository of prescription
        /// </summary>
        private readonly IRepositoryPrescription _repositoryPrescription;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        /// Repository of notification.
        /// </summary>
        private readonly INotificationService _notificationService;

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