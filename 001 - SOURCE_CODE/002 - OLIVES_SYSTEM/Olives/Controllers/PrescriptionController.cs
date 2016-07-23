using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/medical/prescription")]
    public class PrescriptionController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryMedicalRecord"></param>
        /// <param name="repositoryPrescription"></param>
        /// <param name="log"></param>
        public PrescriptionController(IRepositoryAccount repositoryAccount,
            IRepositoryMedicalRecord repositoryMedicalRecord, IRepositoryPrescription repositoryPrescription,
            ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryMedicalRecord = repositoryMedicalRecord;
            _repositoryPrescription = repositoryPrescription;
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
                await _repositoryAccount.FindPersonAsync(prescription.Owner, null, null, null, StatusAccount.Active);
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
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, owner.Id,
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

                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, owner.Id,
                    (byte) StatusRelation.Active);

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

            #region Information update

            try
            {
                var prescription = new Prescription();
                prescription.Owner = medicalRecord.Owner;
                prescription.MedicalRecordId = medicalRecord.Id;
                prescription.From = info.From;
                prescription.To = info.To;
                if (info.Medicines != null)
                    prescription.Medicine = JsonConvert.SerializeObject(info.Medicines);

                prescription.Note = info.Note;
                prescription.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

                // Initialize prescription to database.
                prescription = await _repositoryPrescription.InitializePrescriptionAsync(prescription);


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

            // Requester is different from the medical record owner.
            if (requester.Id != prescription.Owner)
            {
                // Find the owner of medical record.
                var owner =
                    await
                        _repositoryAccount.FindPersonAsync(prescription.Owner, null, null, (byte) Role.Patient,
                            StatusAccount.Active);

                // Owner cannot be found.
                if (owner == null)
                {
                    // Log the error.
                    _log.Error($"Owner [Id: {prescription.Owner}] is not found as active.");

                    // Tell the client about the result.
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnOwnerNotActive}"
                    });
                }

                // Find the relationship between requester and owner.
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, owner.Id,
                    (byte) StatusRelation.Active);

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

            #region Information construction

            try
            {
                if (info.From != null)
                    prescription.From = info.From.Value;

                if (info.To != null)
                    prescription.To = info.To.Value;

                if (info.Medicines != null)
                    prescription.Medicine = JsonConvert.SerializeObject(info.Medicines);

                if (!string.IsNullOrEmpty(info.Note))
                    prescription.Note = info.Note;

                // Update last modified time.
                prescription.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

                // Initialize prescription to database.
                prescription = await _repositoryPrescription.InitializePrescriptionAsync(prescription);

                #endregion

                #region Response initialization

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
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of medical record
        /// </summary>
        private readonly IRepositoryMedicalRecord _repositoryMedicalRecord;

        /// <summary>
        ///     Repository of prescription
        /// </summary>
        private readonly IRepositoryPrescription _repositoryPrescription;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}