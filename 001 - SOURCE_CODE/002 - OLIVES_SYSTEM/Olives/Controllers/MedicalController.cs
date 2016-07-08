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
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
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
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record by using id.
            var results = await _repositoryMedical.FindMedicalRecordAsync(id);

            // No result has been received.
            if (results == null || results.Count != 1)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
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
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is requesting to see the personal note of another person.
            if (requester.Id != result.Owner)
            {
                // Retrieve the relation between these 2 people.
                var relationships =
                    await _repositoryAccount.FindRelation(requester.Id, result.Owner, (byte)StatusRelation.Active);
                var relationship = relationships.FirstOrDefault();

                // There is no relationship between these 2 people
                if (relationship == null)
                {
                    // Tell client no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
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
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
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
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Owner is defined.
            if (info.Owner != null)
            {
                // Requester is requesting to create medical record for another person
                if (requester.Id != info.Owner)
                {
                    // Patient can only create medical record for himself/herself.
                    if (requester.Role == (byte)Role.Patient)
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            Error = $"{Language.WarnRoleIsForbidden}"
                        });

                    // Find the relationship between requester and the record owner.
                    var relationship =
                        await _repositoryAccount.FindRelationParticipation(requester.Id, info.Owner.Value,
                            (byte)StatusRelation.Active);

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
        [OlivesAuthorize(new[] { Role.Patient, Role.Doctor })]
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
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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
                    (byte)StatusAccount.Active);
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
        [OlivesAuthorize(new[] { Role.Patient, Role.Doctor })]
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
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // No owner is specified. That means the requester wants to filter his/her records.
            if (filter.Owner == null)
                filter.Owner = requester.Id;
            else
            {
                // Find the relationship between the requester and the owner.
                // Check the relationship between them.
                var relationship = await _repositoryAccount.FindRelationParticipation(requester.Id, filter.Owner.Value,
                    (byte)StatusAccount.Active);
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
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
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
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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
                if (requester.Role == (byte)Role.Patient)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });

                // Find the relationship between requester and the record owner.
                var relationship = await _repositoryAccount.FindRelationParticipation(requester.Id, medicalRecord.Owner,
                    (byte)StatusRelation.Active);

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
        [OlivesAuthorize(new[] { Role.Patient })]
        public async Task<HttpResponseMessage> DeleteMedicalImage([FromBody] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
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
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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
                if (requester.Role == (byte)Role.Patient)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });

                // Find the relationship between requester and the record owner.
                var relationship = await _repositoryAccount.FindRelationParticipation(requester.Id, medicalRecord.Owner,
                    (byte)StatusRelation.Active);

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

        #region Medical prescription

        [Route("api/medical/prescription")]
        [HttpGet]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> RetrievePrescription([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Prescription validation

            // Find the prescription by using id.
            var prescription = await _repositoryMedical.FindPrescriptionAsync(id);

            // No record is found.
            if (prescription == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Prescription owner validation

            // Find the owner of medical record.
            var owner = await _repositoryAccount.FindPersonAsync(prescription.Owner, null, null, null, StatusAccount.Active);
            if (owner == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnOwnerNotActive}"
                });
            }

            
            // Requester is different from the medical record owner.
            if (requester.Id != owner.Id)
            {
                // Patient cannot give another person prescription.
                if (owner.Role == (byte)Role.Patient)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }

                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationParticipation(requester.Id, owner.Id,
                    (byte)StatusRelation.Active);

                // No active relationship is found.
                if (relationships == null || relationships.Count < 1)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }
            }
            else
            {
                // Doctor cannot create prescription for him/herself.
                if (requester.Role == (byte)Role.Doctor)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }
            }

            #endregion
            
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                prescription.Id,
                MedicalRecord = prescription.MedicalRecordId,
                prescription.From,
                prescription.To,
                prescription.Note
            });
        }

        /// <summary>
        /// Initialize a prescription.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/medical/prescription")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> InitializePrescription([FromBody] InitializePrescriptionViewModel info)
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
            var medicalRecords = await _repositoryMedical.FindMedicalRecordAsync(info.MedicalRecordId);
            if (medicalRecords == null || medicalRecords.Count != 1)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            // Retrieve the first queried medical record.
            var medicalRecord = medicalRecords.FirstOrDefault();
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

            // Find the owner of medical record.
            var owner = await _repositoryAccount.FindPersonAsync(medicalRecord.Owner, null, null, null, StatusAccount.Active);
            if (owner == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnOwnerNotActive}"
                });
            }

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical record owner.
            if (requester.Id != owner.Id)
            {
                // Patient cannot give another person prescription.
                if (owner.Role == (byte)Role.Patient)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }

                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationParticipation(requester.Id, owner.Id,
                    (byte)StatusRelation.Active);

                // No active relationship is found.
                if (relationships == null || relationships.Count < 1)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }
            }
            else
            {
                // Doctor cannot create prescription for him/herself.
                if (requester.Role == (byte)Role.Doctor)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }
            }

            #endregion

            #region Information construction

            var prescription = new Prescription();
            prescription.Owner = owner.Id;
            prescription.MedicalRecordId = medicalRecord.Id;
            prescription.From = info.From;
            prescription.To = info.To;
            prescription.Note = info.Note;
            prescription.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Initialize prescription to database.
            prescription = await _repositoryMedical.InitializePrescriptionAsync(prescription);

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                prescription.Id,
                MedicalRecord = prescription.MedicalRecordId,
                prescription.From,
                prescription.To,
                prescription.Note,
                prescription.Created
            });
        }

        /// <summary>
        /// Edit a prescription.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/medical/prescription")]
        [HttpPut]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> EditPrescription([FromUri] int id, [FromBody] EditPrescriptionViewModel info)
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
            
            #region Prescription owner validation

            // Find the prescription by using id.
            var prescription = await _repositoryMedical.FindPrescriptionAsync(id);
            if (prescription == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Find the owner of medical record.
            var owner = await _repositoryAccount.FindPersonAsync(prescription.Owner, null, null, null, StatusAccount.Active);
            if (owner == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnOwnerNotActive}"
                });
            }

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical record owner.
            if (requester.Id != owner.Id)
            {
                // Patient cannot give another person prescription.
                if (owner.Role == (byte)Role.Patient)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }

                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationParticipation(requester.Id, owner.Id,
                    (byte)StatusRelation.Active);

                // No active relationship is found.
                if (relationships == null || relationships.Count < 1)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }
            }
            else
            {
                // Doctor cannot create prescription for him/herself.
                if (requester.Role == (byte)Role.Doctor)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }
            }

            #endregion

            #region Information construction

            if (info.From != null)
                prescription.From = info.From.Value;

            if (info.To != null)
                prescription.To = info.To.Value;

            if (!string.IsNullOrEmpty(info.Note))
                prescription.Note = info.Note;

            // Update last modified time.
            prescription.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Initialize prescription to database.
            prescription = await _repositoryMedical.InitializePrescriptionAsync(prescription);

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                prescription.Id,
                MedicalRecord = prescription.MedicalRecordId,
                prescription.From,
                prescription.To,
                prescription.Note
            });
        }

        /// <summary>
        /// Edit a prescription.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/medical/prescription")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeletePrescription([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Patient can only delete his/her record.
            var deletedRecords = await _repositoryMedical.DeletePrescriptionAsync(id, requester.Id);

            // No record has been deleted.
            if (deletedRecords < 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Edit a prescription.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/prescription/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> FilterPrescription([FromBody] FilterPrescriptionViewModel filter)
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

            #region Medical record owner validation

            // Find the prescription by using id.
            var medicalRecords = await _repositoryMedical.FindMedicalRecordAsync(filter.MedicalRecord);
            if (medicalRecords == null || medicalRecords.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }
            
            // Retrieve the first queried medical record.
            var medicalRecord = medicalRecords.FirstOrDefault();
            if (medicalRecord == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            // Find the owner of medical record.
            var owner = await _repositoryAccount.FindPersonAsync(medicalRecord.Owner, null, null, null, StatusAccount.Active);
            if (owner == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnOwnerNotActive}"
                });
            }

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical record owner.
            if (requester.Id != owner.Id)
            {
                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationParticipation(requester.Id, owner.Id,
                    (byte)StatusRelation.Active);

                // No active relationship is found.
                if (relationships == null || relationships.Count < 1)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }
            }

            #endregion
            
            // Filter prescription by using specific conditions.
            var result = await _repositoryMedical.FilterPrescriptionAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Prescriptions = result.Prescriptions.Select(x => new
                {
                    x.Id,
                    MedicalRecord = x.MedicalRecordId,
                    x.From,
                    x.To,
                    x.Note,
                    x.Created,
                    x.LastModified
                }),
                result.Total
            });
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