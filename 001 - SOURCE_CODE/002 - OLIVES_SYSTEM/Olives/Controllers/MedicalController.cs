﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.Models;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Olives.ViewModels.Modify;
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
            var result = await _repositoryMedical.FindMedicalRecordAsync(id);

            // No result has been received.
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
                    await
                        _repositoryAccount.FindRelationshipAsync(requester.Id, result.Owner,
                            (byte)StatusRelation.Active);
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
                    result.Owner,
                    result.Creator,
                    result.Info,
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
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Owner of medical record should be the requester.
            if (info.Owner == null)
            {
                info.Owner = requester.Id;

                // Doctors cannot create medical record for themselves.
                if (requester.Role == (byte)Role.Doctor)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }
            }

            // Requester is requesting to create medical record for another person
            if (requester.Id != info.Owner)
            {
                // Patient can only create medical record for himself/herself.
                if (requester.Role == (byte)Role.Patient)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });

                // Find the active patient.
                var owner = await _repositoryAccount.FindPersonAsync(info.Owner, null, null, (byte)Role.Patient,
                    StatusAccount.Active);

                // Owner is not found.
                if (owner == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnOwnerNotActive}"
                    });
                }

                // Find the relationship between requester and the record owner.
                var relationship =
                    await _repositoryAccount.FindRelationshipAsync(requester.Id, info.Owner.Value,
                        (byte)StatusRelation.Active);

                // No relationship is found between 2 people.
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
            }

            // Only filter and receive the first result.
            var medicalRecord = new MedicalRecord();
            medicalRecord.Owner = info.Owner.Value;

            medicalRecord.Info = JsonConvert.SerializeObject(info.Infos);
            medicalRecord.Time = info.Time;
            medicalRecord.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryMedical.InitializeMedicalRecordAsync(medicalRecord);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecord = new
                {
                    result.Id,
                    result.Owner,
                    result.Creator,
                    result.Info,
                    result.Time,
                    result.Created
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
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] EditMedicalRecordViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new EditMedicalRecordViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the record first.
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(id);

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
                // Find the active patient.
                var owner =
                    await _repositoryAccount.FindPersonAsync(medicalRecord.Owner, null, null, (byte)Role.Patient,
                        StatusAccount.Active);

                // Owner is not found.
                if (owner == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnOwnerNotActive}"
                    });
                }

                // Check the relationship between them.
                var relationship = await _repositoryAccount.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
                    (byte)StatusAccount.Active);
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
            }

            // Infos needs updating.
            if (info.Infos != null)
                medicalRecord.Info = JsonConvert.SerializeObject(info.Infos);

            // Time needs updating.
            if (info.Time != null)
                medicalRecord.Time = info.Time.Value;

            // Update the last time
            medicalRecord.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryMedical.InitializeMedicalRecordAsync(medicalRecord);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecord = new
                {
                    result.Id,
                    result.Info,
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

            // Requester is a patient. He/she can only see his/her medical record.
            if (requester.Role == (byte)Role.Patient)
                filter.Owner = requester.Id;
            else
                filter.Creator = requester.Id;

            // Filter medical records.
            var results = await _repositoryMedical.FilterMedicalRecordAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecords = results.MedicalRecords.Select(x => new
                {
                    x.Id,
                    x.Owner,
                    x.Creator,
                    x.Info,
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
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(info.MedicalRecord);
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
                var relationship = await _repositoryAccount.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
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
                var fullPath = Path.Combine(_applicationSetting.PrivateStorage.Absolute,
                    $"{imageName}.{Values.StandardImageExtension}");
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
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(info.MedicalRecord);
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
                var relationship = await _repositoryAccount.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
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
                                File.ReadAllBytes(Path.Combine(_applicationSetting.PrivateStorage.Absolute,
                                    $"{x.Image}.{Values.StandardImageExtension}"))),
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

        /// <summary>
        ///     Retrieve a prescription by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            var owner =
                await _repositoryAccount.FindPersonAsync(prescription.Owner, null, null, null, StatusAccount.Active);
            if (owner == null)
            {
                // Tell requester the record isn't found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is different from the medical record owner.
            if (requester.Id != owner.Id)
            {
                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, owner.Id,
                    (byte)StatusRelation.Active);

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
        }

        /// <summary>
        ///     Initialize a prescription.
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
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(info.MedicalRecord);
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
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical record owner.
            if (requester.Id != medicalRecord.Owner)
            {
                // Find the owner of medical record.
                var owner =
                    await
                        _repositoryAccount.FindPersonAsync(medicalRecord.Owner, null, null, null, StatusAccount.Active);
                if (owner == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnOwnerNotActive}"
                    });
                }

                // Patient cannot give another person prescription.
                if (owner.Role == (byte)Role.Patient)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }

                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, owner.Id,
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
            prescription.Owner = medicalRecord.Owner;
            prescription.MedicalRecordId = medicalRecord.Id;
            prescription.From = info.From;
            prescription.To = info.To;
            if (info.Medicines != null)
                prescription.Medicine = JsonConvert.SerializeObject(info.Medicines);

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
                prescription.Name,
                prescription.Medicine,
                prescription.Note,
                prescription.Created
            });
        }

        /// <summary>
        ///     Edit a prescription.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/medical/prescription")]
        [HttpPut]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> EditPrescription([FromUri] int id,
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
            var owner =
                await _repositoryAccount.FindPersonAsync(prescription.Owner, null, null, null, StatusAccount.Active);
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
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, owner.Id,
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

            if (info.Medicines != null)
                prescription.Medicine = JsonConvert.SerializeObject(info.Medicines);

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
                prescription.Name,
                prescription.Medicine,
                prescription.Note,
                prescription.Created,
                prescription.LastModified
            });
        }

        /// <summary>
        ///     Edit a prescription.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/medical/prescription")]
        [HttpDelete]
        [OlivesAuthorize(new[] { Role.Patient })]
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
        ///     Edit a prescription.
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

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Owner validation

            Person owner;
            if (filter.MedicalRecord != null)
            {
                // Find the prescription by using id.
                var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(filter.MedicalRecord.Value);
                if (medicalRecord == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnMedicalRecordNotFound}"
                    });
                }


                // Find the owner of medical record.
                owner =
                    await
                        _repositoryAccount.FindPersonAsync(medicalRecord.Owner, null, null, null, StatusAccount.Active);
            }
            else
            {
                if (filter.Owner != null)
                    owner = await _repositoryAccount.FindPersonAsync(filter.Owner, null, null, null, StatusAccount.Active);
                else
                    owner = requester;
            }

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
                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, owner.Id,
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
                    x.Name,
                    x.Medicine,
                    x.Note,
                    x.Created,
                    x.LastModified
                }),
                result.Total
            });
        }

        #endregion

        #region Medical prescription image

        /// <summary>
        /// Initialize a prescription image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/medical/prescription/image")]
        public async Task<HttpResponseMessage> InitializePrescriptionImage([FromBody] InitializePrescriptionImageViewModel initializer)
        {
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

            Image medicalImageFile = null;

            // Medical image validation.
            try
            {
                var memoryStream = new MemoryStream(initializer.Image.Buffer);
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

            // Find the medical prescription.
            var prescription = await _repositoryMedical.FindPrescriptionAsync(initializer.Prescription);
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

            // Requester is not the creator of prescription.
            if (requester.Id != prescription.Owner)
            {
                // Log the error first.
                _log.Error($"Requester [Id: {requester.Id}] is different from the Creator [Id: {prescription.Owner}]");

                // Tell the client, the action is forbidden.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotCreator}"
                });
            }

            try
            {
                // Generate file name and save the file first.
                var fileName = Guid.NewGuid().ToString("N");

                // Save the image first.
                var fullPath = Path.Combine(_applicationSetting.PrescriptionStorage.Absolute,
                    $"{fileName}.{Values.StandardImageExtension}");
                medicalImageFile.Save(fullPath);

                // Initialize a prescription image.
                var prescriptionImage = new PrescriptionImage();
                prescriptionImage.Image = fileName;
                prescriptionImage.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
                prescriptionImage.Creator = requester.Id;

                // Save the prescription image to database.
                await _repositoryMedical.InitializePrescriptionImage(prescriptionImage);
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
        /// Initialize a prescription image
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/medical/prescription/image")]
        public async Task<HttpResponseMessage> DeletePrescriptionImage([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical prescription.
            var prescriptionImage = await _repositoryMedical.FindPrescriptionImageAsync(id);
            if (prescriptionImage == null)
            {
                // Writing log.
                _log.Error($"Prescription [{id}] is not found.");

                // Respond the warning to client.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester is not the creator of prescription.
            if (requester.Id != prescriptionImage.Creator)
            {
                // Log the error first.
                _log.Error($"Requester [Id: {requester.Id}] is different from the Creator [Id: {prescriptionImage.Creator}]");

                // Tell the client, the action is forbidden.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotCreator}"
                });
            }

            try
            {
                // Find the prescription image and delete 'em.
                var records = await _repositoryMedical.DeletePrescriptionImageAsync(id);

                if (records < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                try
                {
                    // Save the image first.
                    var fullPath = Path.Combine(_applicationSetting.PrivateStorage.Absolute,
                        $"{prescriptionImage.Image}.{Values.StandardImageExtension}");

                    // Initialize a prescription image.
                    File.Delete(fullPath);
                }
                catch (Exception exception)
                {
                    // File cannot be deleted for a reason.
                    // Log the error and continue.
                    _log.Error(exception.Message, exception);
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
        /// Filter prescription image by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/medical/prescription/image/filter")]
        public async Task<HttpResponseMessage> FilterPrescriptionImage([FromBody] FilterPrescriptionImageViewModel filter)
        {
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

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester initialization.
            filter.Requester = requester.Id;

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

        #endregion

        #region Medical experiment

        /// <summary>
        ///     Initialize a medical experiment note with extra information.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/medical/experiment")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> InitializeMedialExperiment(
            [FromBody] InitializeMedicalExperiment initializer)
        {
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

            // Find the medical record first.
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(initializer.MedicalRecord);

            // Medical record is not found.
            if (medicalRecord == null)
            {
                _log.Error($"Medical record {initializer.MedicalRecord} is not found");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical owner.
            if (requester.Id != medicalRecord.Owner)
            {
                // Patient cannot note experiment result to another person.
                if (requester.Role == (byte)Role.Patient)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }

                // Find the relationship between the requester and prescription owner.
                var relationship =
                    await
                        _repositoryAccount.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
                            (byte)StatusRelation.Active);

                // No relationship is found
                if (relationship == null)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
            }

            // Initialize note.
            var note = new ExperimentNote();
            note.Info = JsonConvert.SerializeObject(initializer.Infos);
            note.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
            note.MedicalRecordId = initializer.MedicalRecord;
            note.Name = initializer.Name;
            note.Owner = medicalRecord.Owner;

            try
            {
                note = await _repositoryMedical.InitializeExperimentNote(note);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Note = new
                    {
                        note.Id,
                        MedicalRecord = note.MedicalRecordId,
                        note.Owner,
                        note.Name,
                        note.Info,
                        note.Created
                    }
                });
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
        }

        /// <summary>
        ///     Initialize a medical experiment note with extra information.
        /// </summary>
        /// <param name="experiment">Experiment which contains records.</param>
        /// <param name="modifier">List of informations which need changing</param>
        /// <returns></returns>
        [Route("api/medical/experiment")]
        [HttpPut]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> ModifyMedialExperimentNote([FromUri] int experiment,
            [FromBody] EditMedicalExperiment modifier)
        {
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

            // Find the medical record first.
            var experimentNote = await _repositoryMedical.FindExperimentNoteAsync(experiment);

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical owner.
            if (requester.Id != experimentNote.Owner)
            {
                // Patient cannot note experiment result to another person.
                if (requester.Role == (byte)Role.Patient)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnRoleIsForbidden}"
                    });
                }

                // Find the relationship between the requester and prescription owner.
                var relationship =
                    await
                        _repositoryAccount.FindRelationshipAsync(requester.Id, experimentNote.Owner,
                            (byte)StatusRelation.Active);

                // No relationship is found
                if (relationship == null)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
            }


            try
            {
                // Name is defined/
                if (!string.IsNullOrWhiteSpace(modifier.Name))
                    experimentNote.Name = modifier.Name;

                // Information is specified.
                if (modifier.Infos != null)
                    experimentNote.Info = JsonConvert.SerializeObject(modifier.Infos);

                // Update the last modified time.
                experimentNote.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
                var failedRecords = await _repositoryMedical.InitializeExperimentNote(experimentNote);

                // No record is failed.
                if (failedRecords == null)
                    return Request.CreateResponse(HttpStatusCode.OK);

                // Send the list of failed record back to client.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnFailedBulkUpdate}",
                    FailedRecords = failedRecords
                });
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
        }

        /// <summary>
        ///     Delete a medical experiment note or only its key-value pairs.
        /// </summary>
        /// <param name="experiment">Experiment which contains records.</param>
        /// <returns></returns>
        [Route("api/medical/experiment")]
        [HttpDelete]
        [OlivesAuthorize(new[] { Role.Patient })]
        public async Task<HttpResponseMessage> DeleteMedialExperimentNote([FromUri] int experiment)
        {
            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Find the medical record first.
            var experimentNote = await _repositoryMedical.FindExperimentNoteAsync(experiment);

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical owner.
            if (requester.Id != experimentNote.Owner)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            try
            {
                // Remove note and retrieve the response.
                var response = await _repositoryMedical.DeleteExperimentNotesAsync(experimentNote.Id);

                // Send the list of failed record back to client.
                return Request.CreateResponse(HttpStatusCode.OK, response);
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
        }

        #endregion

        #region Medical note

        /// <summary>
        ///     Find a medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/medical/note")]
        [HttpGet]
        [OlivesAuthorize(new[] { Role.Doctor })]
        public async Task<HttpResponseMessage> RetrieveMedicalNote([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record by using id.
            var result = await _repositoryMedical.FindMedicalNoteAsync(id);

            // No result has been received.
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
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalNote = new
                {
                    result.Id,
                    MedicalRecord = result.MedicalRecordId,
                    result.Owner,
                    result.Creator,
                    result.Note,
                    result.Time,
                    result.Created,
                    result.LastModified
                }
            });
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/medical/note")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Doctor })]
        public async Task<HttpResponseMessage> InitializeMedicalNote([FromBody] InitializeMedicalNoteViewModel initializer)
        {
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

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record.
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(initializer.MedicalRecord);

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

            // Find the active patient.
            var owner = await _repositoryAccount.FindPersonAsync(medicalRecord.Owner, null, null, (byte)Role.Patient,
                StatusAccount.Active);

            // Owner is not found.
            if (owner == null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnOwnerNotActive}"
                });
            }

            // Find the relationship between requester and the record owner.
            var relationship =
                await _repositoryAccount.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
                    (byte)StatusRelation.Active);

            // No relationship is found between 2 people.
            if (relationship == null || relationship.Count < 1)
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
                });


            // Initialize an instance of MedicalNote.
            var medicalNote = new MedicalNote();
            medicalNote.MedicalRecordId = initializer.MedicalRecord;
            medicalNote.Creator = requester.Id;
            medicalNote.Owner = medicalRecord.Owner;
            medicalNote.Note = initializer.Note;
            medicalNote.Time = initializer.Time;
            medicalNote.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            medicalNote = await _repositoryMedical.InitializeMedicalNoteAsync(medicalNote);

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
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [Route("api/medical/note")]
        [HttpPut]
        [OlivesAuthorize(new[] { Role.Doctor })]
        public async Task<HttpResponseMessage> ModifyMedicalNote([FromUri] int id, [FromBody] EditMedicalNoteViewModel modifier)
        {
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

            // Find the medical note.
            var medicalNote = await _repositoryMedical.FindMedicalNoteAsync(id);

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

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is not the medical note creator.
            if (requester.Id != medicalNote.Creator)
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });

            // Note is defined.
            if (modifier.Note != null)
                medicalNote.Note = modifier.Note;

            // Time is defined.
            if (modifier.Time != null)
                medicalNote.Time = modifier.Time.Value;

            medicalNote.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryMedical.InitializeMedicalNoteAsync(medicalNote);

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
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/note/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Doctor })]
        public async Task<HttpResponseMessage> FilterMedicalNote([FromBody] FilterMedicalNoteViewModel filter)
        {
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

            // Retrieve information of person who sent request.
            var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is a doctor, therefore, he/she is the medical note creator.
            filter.Creator = requester.Id;

            // Insert a new allergy to database.
            var result = await _repositoryMedical.FilterMedicalNotesAsync(filter);

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

        #endregion

        #region Medical category

        /// <summary>
        ///     Find a medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/medical/category")]
        [HttpGet]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> RetrieveCategory([FromUri] int id)
        {
            // Find the category.
            var category = await _repositoryMedical.FindMedicalCategoryAsync(id, null, null);

            // Category is not found.
            if (category == null)
            {
                // Log the error.
                _log.Error($"Category [Id: {id}] is doesn't exist");

                // Respond client.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalCategory = new
                {
                    category.Id,
                    category.Name,
                    category.Created,
                    category.LastModified
                }
            });
        }

        /// <summary>
        /// Filter medical categories asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/category/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> FilterCategories([FromBody] FilterMedicalCategoryViewModel filter)
        {
            // Filter hasn't been initialized before.
            if (filter == null)
            {
                filter = new FilterMedicalCategoryViewModel();
                Validate(filter);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error and respond client.
                _log.Error("Request parameters are invalid. Errors sent to client.");

                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Do the filter.
            var result = await _repositoryMedical.FilterMedicalCategoryAsync(filter);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalCategories = result.MedicalCategories.Select(x => new
                {
                    x.Id,
                    x.Name,
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