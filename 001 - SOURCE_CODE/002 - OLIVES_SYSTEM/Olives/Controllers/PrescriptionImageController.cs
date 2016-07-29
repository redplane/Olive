using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Models;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Initialize;

namespace Olives.Controllers
{
    [Route("api/medical/prescription/image")]
    public class PrescriptionImageController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryPrescription"></param>
        /// <param name="repositoryPrescriptionImage"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="log"></param>
        /// <param name="fileService"></param>
        /// <param name="timeService"></param>
        /// <param name="applicationSetting"></param>
        public PrescriptionImageController(IRepositoryAccount repositoryAccount,
            IRepositoryPrescription repositoryPrescription, IRepositoryPrescriptionImage repositoryPrescriptionImage,
            IRepositoryRelation repositoryRelation,
            ILog log, IFileService fileService, ITimeService timeService, ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryPrescription = repositoryPrescription;
            _repositoryPrescriptionImage = repositoryPrescriptionImage;
            _repositoryRelation = repositoryRelation;
            _log = log;
            _fileService = fileService;
            _timeService = timeService;
            _applicationSetting = applicationSetting;
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

            // Requester is not the creator of prescription.
            if (requester.Id != prescription.Owner)
            {
                // Find the owner.
                var owner = _repositoryAccount.FindPersonAsync(prescription.Owner, null, null, null,
                    StatusAccount.Active);

                // No active owner is found.
                if (owner == null)
                {
                    // Log the error for future tracking.
                    _log.Error($"Owner [Id: {prescription.Owner}] is not found");

                    // Tell the client to try again.
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnOwnerNotActive}"
                    });
                }

                // Find the relationship between requester and the record owner.
                var relationship = await _repositoryRelation.FindRelationshipAsync(requester.Id, prescription.Owner,
                    (byte) StatusRelation.Active);

                // No relationship is found between 2 people.
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
            }

            #endregion

            #region Image upload

            try
            {
                // Generate file name and save the file first.
                var fileName = Guid.NewGuid().ToString("N");

                // Full path construction.
                var fullPath = Path.Combine(_applicationSetting.PrescriptionStorage.Absolute,
                    $"{fileName}.{Values.StandardImageExtension}");

                // Save the image first.
                initializer.Image.Save(fullPath);

                // Initialize a prescription image.
                var prescriptionImage = new PrescriptionImage();
                prescriptionImage.PrescriptionId = prescription.Id;
                prescriptionImage.Image = fileName;
                prescriptionImage.FullPath = fullPath;
                prescriptionImage.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                prescriptionImage.Creator = requester.Id;
                prescriptionImage.Owner = prescription.Owner;

                // Save the prescription image to database.
                await _repositoryPrescriptionImage.InitializePrescriptionImage(prescriptionImage);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    PrescriptionImage = new
                    {
                        prescriptionImage.Created,
                        prescriptionImage.Creator,
                        prescriptionImage.Owner
                    }
                });
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
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
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
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of prescription images.
        /// </summary>
        private readonly IRepositoryPrescriptionImage _repositoryPrescriptionImage;

        /// <summary>
        ///     Repository of prescriptions.
        /// </summary>
        private readonly IRepositoryPrescription _repositoryPrescription;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Service which provides functions to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

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