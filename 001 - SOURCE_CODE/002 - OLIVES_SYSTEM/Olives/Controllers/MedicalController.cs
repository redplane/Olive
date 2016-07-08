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
    public class MedicalController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryMedical"></param>
        /// <param name="log"></param>
        /// <param name="applicationSetting"></param>
        public MedicalController(IRepositoryAccount repositoryAccount, IRepositoryMedical repositoryMedical,
            ILog log, ApplicationSetting applicationSetting)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryMedical = repositoryMedical;
            _log = log;
            _applicationSetting = applicationSetting;
        }

        #endregion

        #region Methods

        #region Medical record

        /// <summary>
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/medical/record")]
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record by using id.
            var results = await _repositoryMedical.FindMedicalRecordAsync(id);

            // No result has been received.
            if (results == null || results.Count != 1)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnNoRecord}"
                });
            }

            // 
            // Retrieve the first queried result.
            var result = results.FirstOrDefault();
            if (result == null)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnNoRecord}"
                });
            }

            // Requester is requesting to see the personal note of another person.
            if (requester.Id != result.Owner)
            {
                // Retrieve the relation between these 2 people.
                var relationships =
                    await _repositoryAccount.FindRelation(requester.Id, result.Owner, (byte) StatusRelation.Active);
                var relationship = relationships.FirstOrDefault();

                // There is no relationship between these 2 people
                if (relationship == null)
                {
                    // Tell client no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnNoRecord}"
                    });
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecord = new
                {
                    result.Id,
                    result.Summary,
                    result.Tests,
                    result.AdditionalMorbidities,
                    result.DifferentialDiagnosis,
                    result.OtherPathologies,
                    result.Time,
                    result.Created,
                    result.LastModified
                }
            });
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/medical/record")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeMedicalRecordViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new InitializeMedicalRecordViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid medical record request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Owner is defined.
            if (info.Owner != null)
            {
                // Requester is requesting to create medical record for another person
                if (requester.Id != info.Owner)
                {
                    // Patient can only create medical record for himself/herself.
                    if (requester.Role == (byte) Role.Patient)
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            Error = $"{Language.WarnRoleIsForbidden}"
                        });

                    // Find the relationship between requester and the record owner.
                    var relationship =
                        await _repositoryAccount.FindRelationParticipation(requester.Id, info.Owner.Value,
                            (byte) StatusRelation.Active);

                    // No relationship is found between 2 people.
                    if (relationship == null || relationship.Count < 1)
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            Error = $"{Language.WarnRelationNotExist}"
                        });
                }
            }

            // Only filter and receive the first result.
            var medicalRecord = new MedicalRecord();
            medicalRecord.Owner = info.Owner ?? requester.Id;
            medicalRecord.Summary = info.Summary;
            medicalRecord.Tests = info.Tests;
            medicalRecord.AdditionalMorbidities = info.AdditionalMorbidities;
            medicalRecord.DifferentialDiagnosis = info.DifferentialDiagnosis;
            medicalRecord.OtherPathologies = info.OtherPathologies;
            medicalRecord.Time = info.Time;
            medicalRecord.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryMedical.InitializeMedicalRecordAsync(medicalRecord);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecord = new
                {
                    result.Id,
                    result.Summary,
                    result.Tests,
                    result.AdditionalMorbidities,
                    result.DifferentialDiagnosis,
                    result.OtherPathologies,
                    result.Time,
                    result.Created,
                    result.LastModified
                }
            });
        }

        /// <summary>
        ///     Edit an addiction asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/medical/record")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] InitializeMedicalRecordViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new InitializeMedicalRecordViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid medical record request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the record first.
            var medicalRecords = await _repositoryMedical.FindMedicalRecordAsync(id);

            // Medical record is not found.
            if (medicalRecords == null || medicalRecords.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // No result has been retrieved.
            var medicalRecord = medicalRecords.FirstOrDefault();
            if (medicalRecord == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Owner is different from requester.
            if (medicalRecord.Owner != requester.Id)
            {
                // Check the relationship between them.
                var relationship = await _repositoryAccount.FindRelationParticipation(requester.Id, medicalRecord.Owner,
                    (byte) StatusAccount.Active);
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRelationNotExist}"
                    });
            }

            // Only filter and receive the first result.
            if (info.Summary != null)
                medicalRecord.Summary = info.Summary;

            if (info.Tests != null)
                medicalRecord.Tests = info.Tests;

            if (info.AdditionalMorbidities != null)
                medicalRecord.AdditionalMorbidities = info.AdditionalMorbidities;

            if (info.DifferentialDiagnosis != null)
                medicalRecord.DifferentialDiagnosis = info.DifferentialDiagnosis;

            if (info.OtherPathologies != null)
                medicalRecord.OtherPathologies = info.OtherPathologies;

            medicalRecord.Time = info.Time;
            medicalRecord.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryMedical.InitializeMedicalRecordAsync(medicalRecord);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecord = new
                {
                    result.Id,
                    result.Summary,
                    result.Tests,
                    result.AdditionalMorbidities,
                    result.DifferentialDiagnosis,
                    result.OtherPathologies,
                    result.Time,
                    result.Created,
                    result.LastModified
                }
            });
        }

        [Route("api/medical/record/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> FilterMedicalRecord([FromBody] FilterMedicalRecordViewModel filter)
        {
            // Model hasn't been initialized.
            if (filter == null)
            {
                // Initialize it and do the validation.
                filter = new FilterMedicalRecordViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid medical record request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // No owner is specified. That means the requester wants to filter his/her records.
            if (filter.Owner == null)
                filter.Owner = requester.Id;
            else
            {
                // Find the relationship between the requester and the owner.
                // Check the relationship between them.
                var relationship = await _repositoryAccount.FindRelationParticipation(requester.Id, filter.Owner.Value,
                    (byte) StatusAccount.Active);
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRelationNotExist}"
                    });
            }

            // Filter medical records.
            var results = await _repositoryMedical.FilterMedicalRecordAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecords = results.MedicalRecords.Select(x => new
                {
                    x.Id,
                    x.Owner,
                    x.Summary,
                    x.Tests,
                    x.AdditionalMorbidities,
                    x.DifferentialDiagnosis,
                    x.OtherPathologies,
                    x.Time,
                    x.Created,
                    x.LastModified
                }),
                results.Total
            });
        }

        #endregion

        #region Medical image

        /// <summary>
        ///     Initialize a  asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/medical/image")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializeMedicalImage([FromBody] InitializeMedicalImageViewModel info)
        {
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

            Image medicalImageFile = null;

            // Medical image validation.
            try
            {
                var memoryStream = new MemoryStream(info.File.Buffer);
                memoryStream.Seek(0, SeekOrigin.Begin);
                medicalImageFile = Image.FromStream(memoryStream);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnImageIncorrectFormat}"
                });
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record.
            var medicalRecords = await _repositoryMedical.FindMedicalRecordAsync(info.MedicalRecord);
            if (medicalRecords == null || medicalRecords.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first queried result.
            var medicalRecord = medicalRecords.FirstOrDefault();
            if (medicalRecord == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is requesting to create medical record for another person
            if (requester.Id != medicalRecord.Owner)
            {
                // Patient can only create medical record for himself/herself.
                if (requester.Role == (byte) Role.Patient)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });

                // Find the relationship between requester and the record owner.
                var relationship = await _repositoryAccount.FindRelationParticipation(requester.Id, medicalRecord.Owner,
                    (byte) StatusRelation.Active);

                // No relationship is found between 2 people.
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRelationNotExist}"
                    });
            }

            try
            {
                var medicalImage = new MedicalImage();
                medicalImage.Owner = medicalRecord.Owner;
                medicalImage.MedicalRecordId = medicalRecord.Id;

                var imageName = Guid.NewGuid().ToString("N");
                medicalImage.Image = imageName;
                medicalImage.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

                // Convert image to base64.
                var base64Image = Convert.ToBase64String(info.File.Buffer);

                // Save the image first.
                var fullPath = Path.Combine(_applicationSetting.PrivateStorage.Absolute, $"{imageName}.{Values.StandardImageExtension}");
                medicalImageFile.Save(fullPath);

                // Save the medical record to database.
                await _repositoryMedical.InitializeMedicalImageAsync(medicalImage);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    medicalImage.Id,
                    MedicalRecord = medicalImage.MedicalRecordId,
                    medicalImage.Owner,
                    Image = base64Image,
                    medicalImage.Created
                });
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        /// <summary>
        ///     Delete a medical image asynchronously.
        ///     Only patient can delete the record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/medical/image")]
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeleteMedicalImage([FromBody] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Remove the addiction of the requester.
                var affectedRecords = await _repositoryMedical.DeleteMedicalImageAsync(id, requester.Id);

                // No record has been affected.
                if (affectedRecords < 1)
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
                // Log the exception to file.
                _log.Error(exception.Message, exception);

                // Tell the client that something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        [Route("api/medical/image/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterMedicalImage([FromBody] FilterMedicalImageViewModel info)
        {
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

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record.
            var medicalRecords = await _repositoryMedical.FindMedicalRecordAsync(info.MedicalRecord);
            if (medicalRecords == null || medicalRecords.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first queried result.
            var medicalRecord = medicalRecords.FirstOrDefault();
            if (medicalRecord == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is requesting to create medical record for another person
            if (requester.Id != medicalRecord.Owner)
            {
                // Patient can only create medical record for himself/herself.
                if (requester.Role == (byte) Role.Patient)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });

                // Find the relationship between requester and the record owner.
                var relationship = await _repositoryAccount.FindRelationParticipation(requester.Id, medicalRecord.Owner,
                    (byte) StatusRelation.Active);

                // No relationship is found between 2 people.
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRelationNotExist}"
                    });
            }

            try
            {
                // Filter medical images by using specific conditions.
                var results = await _repositoryMedical.FilterMedicalImageAsync(info);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalImages = results.MedicalImages.Select(x => new
                    {
                        x.Id,
                        x.Created,
                        Image =
                            Convert.ToBase64String(
                                File.ReadAllBytes(Path.Combine(_applicationSetting.PrivateStorage.Absolute, $"{x.Image}.{Values.StandardImageExtension}"))),
                        x.Owner,
                        MedicalRecord = x.MedicalRecordId
                    }),
                    results.Total
                });
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        #endregion

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

        private readonly ApplicationSetting _applicationSetting;

        #endregion
    }
}