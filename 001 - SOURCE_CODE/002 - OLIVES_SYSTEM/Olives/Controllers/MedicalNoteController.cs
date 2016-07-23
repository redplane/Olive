using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
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

namespace Olives.Controllers
{
    [Route("api/medical/note")]
    public class MedicalNoteController : ApiParentController
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
        public MedicalNoteController(IRepositoryAccount repositoryAccount, IRepositoryMedical repositoryMedical,
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
        ///     Find a medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns> 
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FindMedicalNoteAsync([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Find the medical record by using id.
                var result = await _repositoryMedical.FindMedicalNoteAsync(id);

                // No result has been received.
                if (result == null)
                {
                    // Log the error.
                    _log.Error($"Medical note [Id: {id}] is not found");

                    // Tell client no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Requester is requesting to see the personal note of another person.
                if (requester.Id != result.Owner)
                {
                    // Beside owner, only creator can only see the medical record.
                    if (requester.Id != result.Creator)
                    {
                        // Log the error first.
                        _log.Error($"Requester [Id: {requester.Id}] is not the creator of medical note");

                        // Tell client there is no result for him/her to see.
                        return Request.CreateResponse(HttpStatusCode.NotFound, new
                        {
                            Error = $"{Language.WarnRecordNotFound}"
                        });
                    }

                    // Find the relationship between the requester and medical record owner.
                    var relationships =
                        await
                            _repositoryAccount.FindRelationshipAsync(requester.Id, result.Owner,
                                (byte) StatusRelation.Active);

                    // There is no active relationship between the requester and owner.
                    if (relationships == null || relationships.Count < 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new
                        {
                            Error = $"{Language.WarnRecordNotFound}"
                        });
                    }
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
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> InitializeMedicalNoteAsync(
            [FromBody] InitializeMedicalNoteViewModel initializer)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

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

            #endregion

            #region Owner & relationship validation
            
            // Find the active patient.
            var owner =
                await _repositoryAccount.FindPersonAsync(medicalRecord.Owner, null, null, (byte) Role.Patient,
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
                    (byte) StatusRelation.Active);

            // No relationship is found between 2 people.
            if (relationship == null || relationship.Count < 1)
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
                });
            
            #endregion

            try
            {
                // Initialize an instance of MedicalNote.
                var medicalNote = new MedicalNote();
                medicalNote.MedicalRecordId = initializer.MedicalRecord;
                medicalNote.Creator = requester.Id;
                medicalNote.Owner = medicalRecord.Owner;
                medicalNote.Note = initializer.Note;
                medicalNote.Time = initializer.Time;
                medicalNote.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

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
            catch (Exception exception)
            {
                // As the exception happens, log the error first.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> ModifyMedicalNote([FromUri] int id,
            [FromBody] EditMedicalNoteViewModel modifier)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Paramters validation

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

            #region Medical note

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

            #endregion
            
            // Find the relationship between the requester and owner.
            var relationships =
                await
                    _repositoryAccount.FindRelationshipAsync(requester.Id, medicalNote.Owner,
                        (byte) StatusRelation.Active);

            // No relationship is found.
            if (relationships == null || relationships.Count < 1)
            {
                // Log the error.
                _log.Error(
                    $"There is no active relationship between Requester[Id: {requester.Id}] and Owner[Id: {medicalNote.Owner}]");

                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
                });
            }
            
            #region Information update

            try
            {
                // Note is defined.
                if (modifier.Note != null)
                    medicalNote.Note = modifier.Note;

                // Time is defined.
                if (modifier.Time != null)
                    medicalNote.Time = modifier.Time.Value;

                medicalNote.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

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
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/note/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterMedicalNote([FromBody] FilterMedicalNoteViewModel filter)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Parameters validation

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
                filter.Requester = requester.Id;

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