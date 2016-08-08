using System;
using System.Drawing.Imaging;
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
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;
using Olives.Controllers;
using Olives.Enumerations;
using Olives.Interfaces.Medical;

namespace Olives.Controllers
{
    [Route("api/medical/prescription/image")]
    public class PrescriptionImageController : ApiParentControllerHub<NotificationHub>
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryPrescription"></param>
        /// <param name="repositoryPrescriptionImage"></param>
        /// <param name="repositoryStorage"></param>
        /// <param name="log"></param>
        /// <param name="fileService"></param>
        /// <param name="timeService"></param>
        /// <param name="notificationService"></param>
        public PrescriptionImageController(
            IRepositoryPrescription repositoryPrescription, IRepositoryPrescriptionImage repositoryPrescriptionImage,
            IRepositoryStorage repositoryStorage,
            ILog log, 
            IFileService fileService, ITimeService timeService, INotificationService notificationService)
        {
            _repositoryPrescription = repositoryPrescription;
            _repositoryPrescriptionImage = repositoryPrescriptionImage;
            _repositoryStorage = repositoryStorage;
            _log = log;
            _fileService = fileService;
            _timeService = timeService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initialize a prescription image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializePrescriptionImage(
            [FromBody] InitializePrescriptionImageViewModel initializer)
        {
            #region Request parameters validation

            // Initialize hasn't been initialized.
            if (initializer == null)
            {
                // Initialize it.
                initializer = new InitializePrescriptionImageViewModel();

                // Do validation.
                Validate(initializer);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client.");

                // Respond status back to server.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Prescription validation

            // Find the medical prescription.
            var prescription = await _repositoryPrescription.FindPrescriptionAsync(initializer.Prescription);

            // Prescription is not found.
            if (prescription == null)
            {
                // Writing log.
                _log.Error($"Prescription [{initializer.Prescription}] is not found.");

                // Respond the warning to client.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            if (requester.Id != prescription.Owner && requester.Id != prescription.Creator)
            {
                _log.Error($"Requester [Id: {requester.Id}] is not either creator or owner of Prescription [Id: {prescription.Id}]");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Image upload

            try
            {
                #region File initialization

                // Generate file name and save the file first.
                var fileName = Guid.NewGuid().ToString("N");

                // Find the storage of prescription image.
                var storagePrescriptionImage = _repositoryStorage.FindStorage(Storage.PrescriptionImage);

                // Full path construction.
                var fullPath = Path.Combine(storagePrescriptionImage.Absolute,
                    $"{fileName}.{Values.StandardImageExtension}");

                // Convert by stream to image.
                var prescriptionImageFile = _fileService.ConvertBytesToImage(initializer.Image.Buffer);
                // Save the image first.
                prescriptionImageFile.Save(fullPath, ImageFormat.Png);

                #endregion

                #region Result initialization

                // Initialize a prescription image.
                var prescriptionImage = new PrescriptionImage();
                prescriptionImage.PrescriptionId = prescription.Id;
                prescriptionImage.Image = fileName;
                prescriptionImage.FullPath = fullPath;
                prescriptionImage.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                prescriptionImage.Creator = requester.Id;
                prescriptionImage.Owner = prescription.Owner;

                // Save the prescription image to database.
                prescriptionImage = await _repositoryPrescriptionImage.InitializePrescriptionImage(prescriptionImage);

                #endregion
                
                #region Notification broadcast

                if (prescriptionImage.Creator != prescriptionImage.Owner)
                {
                    var recipient = prescriptionImage.Owner;
                    if (requester.Id == prescriptionImage.Owner)
                        recipient = prescriptionImage.Creator;

                    var notification = new Notification();
                    notification.Type = (byte)NotificationType.Create;
                    notification.Topic = (byte)NotificationTopic.PrescriptionImage;
                    notification.Broadcaster = requester.Id;
                    notification.Recipient = recipient;
                    notification.Record = prescriptionImage.Id;
                    notification.Message = string.Format(Language.NotifyPrescriptionImageCreate, requester.FullName);
                    notification.Created = prescriptionImage.Created;

                    // Broadcast the notification with fault tolerant.
                    await _notificationService.BroadcastNotificationAsync(notification, Hub);
                }

                #endregion
                
                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    PrescriptionImage = new
                    {
                        prescriptionImage.Created,
                        prescriptionImage.Creator,
                        prescriptionImage.Owner
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // Something is wrong with server.
                _log.Error(exception.Message, exception);

                // Tell the client there is something wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }

            #endregion
        }

        /// <summary>
        ///     Delete a prescription image
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeletePrescriptionImage([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Find the prescription image and delete 'em.
                var records = await _repositoryPrescriptionImage.DeletePrescriptionImageAsync(id, requester.Id);

                if (records < 0)
                {
                    // Log the error.
                    _log.Error($"No record [Id: {id}] has been found.");

                    // Tell the client about the result.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Something is wrong with server.
                _log.Error(exception.Message, exception);

                // Tell the client there is something wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        /// <summary>
        ///     Filter prescription image by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/medical/prescription/image/filter")]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterPrescriptionImage(
            [FromBody] FilterPrescriptionImageViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                // Initialize it.
                filter = new FilterPrescriptionImageViewModel();

                // Do the validation.
                Validate(filter);
            }

            // Request paramters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client.");

                // Respond result to client.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Filter adjustment

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester initialization.
            filter.Requester = requester.Id;

            #endregion

            #region Result handling

            try
            {
                // Do the filter.
                var result = await _repositoryPrescriptionImage.FilterPrescriptionImageAsync(filter);

                // Respond filtered results to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    PrescriptionImages = result.PrescriptionImages.Select(x => new
                    {
                        x.Id,
                        Prescription = x.PrescriptionId,
                        x.Owner,
                        x.Creator,
                        Image = _fileService.EncodeFileBase64(x.FullPath),
                        x.Created
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // Log the error.
                _log.Error(exception.Message, exception);

                // Tell client about the terminated process.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties
        
        /// <summary>
        ///     Repository of prescription images.
        /// </summary>
        private readonly IRepositoryPrescriptionImage _repositoryPrescriptionImage;

        /// <summary>
        ///     Repository of prescriptions.
        /// </summary>
        private readonly IRepositoryPrescription _repositoryPrescription;

        /// <summary>
        /// Storage of prescription image.
        /// </summary>
        private readonly IRepositoryStorage _repositoryStorage;

        /// <summary>
        ///     Service which provides functions to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        /// Service which provides functions for accessing notification.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;
        
        /// <summary>
        ///     Service which provides functions to handle file operations.
        /// </summary>
        private readonly IFileService _fileService;

        #endregion
    }
}