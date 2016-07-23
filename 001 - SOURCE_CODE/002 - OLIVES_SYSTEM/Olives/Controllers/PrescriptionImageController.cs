using System;
using System.Drawing;
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
using Shared.Helpers;
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
        /// <param name="repositoryMedical"></param>
        /// <param name="log"></param>
        /// <param name="fileService"></param>
        /// <param name="applicationSetting"></param>
        public PrescriptionImageController(IRepositoryAccount repositoryAccount, IRepositoryMedical repositoryMedical,
            ILog log, IFileService fileService, ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryMedical = repositoryMedical;
            _log = log;
            _fileService = fileService;
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

            #region Image validation

            Image prescriptionImageFile = null;

            // Medical image validation.
            try
            {
                var memoryStream = new MemoryStream(initializer.Image.Buffer);
                memoryStream.Seek(0, SeekOrigin.Begin);
                prescriptionImageFile = Image.FromStream(memoryStream);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnImageIncorrectFormat}"
                });
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Prescription validation

            // Find the medical prescription.
            var prescription = await _repositoryMedical.FindPrescriptionAsync(initializer.Prescription);

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
                // Find the relationship between requester and the record owner.
                var relationship = await _repositoryAccount.FindRelationshipAsync(requester.Id, prescription.Owner,
                    (byte) StatusRelation.Active);

                // No relationship is found between 2 people.
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRelationNotExist}"
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
                prescriptionImageFile.Save(fullPath);

                // Initialize a prescription image.
                var prescriptionImage = new PrescriptionImage();
                prescriptionImage.Image = fileName;
                prescriptionImage.FullPath = fullPath;
                prescriptionImage.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
                prescriptionImage.Creator = requester.Id;

                // Save the prescription image to database.
                await _repositoryMedical.InitializePrescriptionImage(prescriptionImage);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    PrescriptionImage = new
                    {
                        Image = fileName,
                        prescriptionImage.Created,
                        prescriptionImage.Creator
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
        ///     Initialize a prescription image
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
                var records = await _repositoryMedical.DeletePrescriptionImageAsync(id, requester.Id);

                if (records < 0)
                {
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

            try
            {
                // Do the filter.
                var result = await _repositoryMedical.FilterPrescriptionImageAsync(filter);

                // Respond filtered results to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    PrescriptionImages = result.PrescriptionImages.Select(x => new
                    {
                        x.Id,
                        Prescription = x.PrescriptionId,
                        x.Owner,
                        x.Creator,
                        Image = Convert.ToBase64String(
                            File.ReadAllBytes(Path.Combine(_applicationSetting.PrescriptionStorage.Absolute,
                                $"{x.Image}.{Values.StandardImageExtension}"))),
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
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of allergies
        /// </summary>
        private readonly IRepositoryMedical _repositoryMedical;

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