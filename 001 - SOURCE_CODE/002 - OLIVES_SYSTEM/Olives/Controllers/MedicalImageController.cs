using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Hubs;
using Olives.Interfaces;
using Olives.Models;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;
using Olives.Controllers;
using Olives.ViewModels.Initialize;

namespace Olives.Controllers
{
    [Route("api/medical/image")]
    public class MedicalImageController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryMedicalRecord"></param>
        /// <param name="repositoryMedicalImage"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        /// <param name="fileService"></param>
        /// <param name="notificationService"></param>
        /// <param name="applicationSetting"></param>
        public MedicalImageController(
            IRepositoryMedicalRecord repositoryMedicalRecord, IRepositoryMedicalImage repositoryMedicalImage,
            ITimeService timeService, INotificationService notificationService,
            ILog log, IFileService fileService, ApplicationSetting applicationSetting)
        {
            _repositoryMedicalRecord = repositoryMedicalRecord;
            _repositoryMedicalImage = repositoryMedicalImage;
            _log = log;
            _timeService = timeService;
            _notificationService = notificationService;
            _fileService = fileService;
            _applicationSetting = applicationSetting;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initialize a  asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializeMedicalImage([FromBody] InitializeMedicalImageViewModel info)
        {
            #region Parameters validation

            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new InitializeMedicalImageViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid medical record request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Medical record validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record.
            var medicalRecord = await _repositoryMedicalRecord.FindMedicalRecordAsync(info.MedicalRecord);
            if (medicalRecord == null)
            {
                _log.Error($"Medical record [Id: {info.MedicalRecord}] is not found");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            // Requester doesn't take part in medical record.
            if (requester.Id != medicalRecord.Owner && requester.Id != medicalRecord.Creator)
            {
                _log.Error($"Requester [Id: {requester.Id}] isn't either medical record creator [Id: {medicalRecord.Creator}] and medical record owner [Id: {medicalRecord.Owner}]");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Information upload

            try
            {
                #region Record initialization

                // Use GUID to randomize image file name.
                var imageName = Guid.NewGuid().ToString("N");

                // Initialize an instance of medical image.
                var medicalImage = new MedicalImage();
                medicalImage.Creator = requester.Id;
                medicalImage.Owner = medicalRecord.Owner;
                medicalImage.MedicalRecordId = medicalRecord.Id;
                medicalImage.Image = imageName;
                medicalImage.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                medicalImage.FullPath = Path.Combine(_applicationSetting.MedicalImageStorage.Absolute,
                    $"{imageName}.{Values.StandardImageExtension}");

                // Save the image first.
                info.File.Save(medicalImage.FullPath);

                // Update image full path.
                // Save the medical record to database.
                medicalImage = await _repositoryMedicalImage.InitializeMedicalImageAsync(medicalImage);

                #endregion
                
                #region Notification broadcast

                if (medicalImage.Creator != medicalImage.Owner)
                {
                    var recipient = medicalRecord.Owner;
                    if (requester.Id == medicalImage.Owner)
                        recipient = medicalImage.Creator;

                    var notification = new Notification();
                    notification.Type = (byte) NotificationType.Create;
                    notification.Topic = (byte) NotificationTopic.MedicalImage;
                    notification.Broadcaster = requester.Id;
                    notification.Recipient = recipient;
                    notification.Record = medicalImage.Id;
                    notification.Message = string.Format(Language.NotifyPrescriptionCreate, requester.FullName);
                    notification.Created = medicalImage.Created;

                    // Broadcast the notification with fault tolerant.
                    await _notificationService.BroadcastNotificationAsync(notification, Hub);
                }

                #endregion
                
                #region Result handling

                // Tell the client about the result of upload.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalImage = new
                    {
                        medicalImage.Id,
                        MedicalRecord = medicalImage.MedicalRecordId,
                        medicalImage.Creator,
                        medicalImage.Owner,
                        medicalImage.Created
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Log the exception first.
                _log.Error(exception.Message, exception);

                // Tell client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Delete a medical image asynchronously.
        ///     Only patient can delete the record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeleteMedicalImage([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Remove the addiction of the requester.
                var records = await _repositoryMedicalImage.DeleteMedicalImageAsync(id, requester.Id);

                // No record has been affected.
                if (records < 1)
                {
                    // Log the error for future tracking.
                    _log.Error($"No record [Id: {id}] is found.");

                    // Tell the client no record has been found.
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
        ///     Filter medical images by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/medical/image/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterMedicalImage([FromBody] FilterMedicalImageViewModel info)
        {
            #region Request parameters validation

            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new FilterMedicalImageViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid filter medical image request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Information filter.

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
            info.Requester = requester.Id;

            try
            {
                // Filter medical images by using specific conditions.
                var results = await _repositoryMedicalImage.FilterMedicalImageAsync(info);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalImages = results.MedicalImages.Select(x => new
                    {
                        x.Id,
                        x.Created,
                        Image = _fileService.EncodeFileBase64(x.FullPath),
                        x.Owner,
                        MedicalRecord = x.MedicalRecordId
                    }),
                    results.Total
                });
            }
            catch (Exception exception)
            {
                // Log the exception first.
                _log.Error(exception.Message, exception);

                // Tell client there is something wrong with the server.
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
        ///     Repository of medical image.
        /// </summary>
        private readonly IRepositoryMedicalImage _repositoryMedicalImage;
        
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

        /// <summary>
        ///     Application setting.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

        /// <summary>
        ///     Service which provides functions to handle file operations.
        /// </summary>
        private readonly IFileService _fileService;

        #endregion
    }
}