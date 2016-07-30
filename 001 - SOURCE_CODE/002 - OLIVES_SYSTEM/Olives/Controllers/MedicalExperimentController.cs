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
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/medical/experiment")]
    public class MedicalExperimentController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryMedicalRecord"></param>
        /// <param name="repositoryExperimentNote"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="log"></param>
        public MedicalExperimentController(IRepositoryMedicalRecord repositoryMedicalRecord,
            IRepositoryExperimentNote repositoryExperimentNote,
            IRepositoryRelation repositoryRelation, ITimeService timeService,
            ILog log)
        {
            _repositoryMedicalRecord = repositoryMedicalRecord;
            _repositoryExperimentNote = repositoryExperimentNote;
            _repositoryRelation = repositoryRelation;
            _timeService = timeService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initialize a medical experiment note with extra information.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> InitializeMedialExperiment(
            [FromBody] InitializeMedicalExperiment initializer)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            #region Parameters validation

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

            #endregion

            #region Medical record validation

            // Find the medical record first.
            var medicalRecord = await _repositoryMedicalRecord.FindMedicalRecordAsync(initializer.MedicalRecord);

            // Medical record is not found.
            if (medicalRecord == null)
            {
                _log.Error($"Medical record {initializer.MedicalRecord} is not found");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnMedicalRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            // Requester is different from the medical owner.
            if (requester.Id != medicalRecord.Owner)
            {
                // Find the relationship between the requester and prescription owner.
                var relationship =
                    await
                        _repositoryRelation.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
                            (byte) StatusRelation.Active);

                // No relationship is found
                if (relationship == null)
                {
                    // Log the error first.
                    _log.Error(
                        $"Requester [Id: {requester.Id}] doesn't have relationship with Medical Record [Owner: {medicalRecord.Owner}]");

                    // Tell the client the request is forbidden
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }
            }

            #endregion

            #region Record initialization

            try
            {
                // Initialize note.
                var note = new ExperimentNote();
                note.Info = JsonConvert.SerializeObject(initializer.Infos);
                note.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                note.MedicalRecordId = initializer.MedicalRecord;
                note.Name = initializer.Name;
                note.Owner = medicalRecord.Owner;
                note.Creator = requester.Id;

                note = await _repositoryExperimentNote.InitializeExperimentNote(note);
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

            #endregion
        }

        /// <summary>
        ///     Initialize a medical experiment note with extra information.
        /// </summary>
        /// <param name="experiment">Experiment which contains records.</param>
        /// <param name="modifier">List of informations which need changing</param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> ModifyMedialExperimentNote([FromUri] int experiment,
            [FromBody] EditMedicalExperiment modifier)
        {
            #region Parameters validation

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

            #endregion

            #region Role validation

            // Find the medical record first.
            var experimentNote = await _repositoryExperimentNote.FindExperimentNoteAsync(experiment);

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical owner.
            if (requester.Id != experimentNote.Owner)
            {
                // Find the relationship between the requester and prescription owner.
                var relationship =
                    await
                        _repositoryRelation.FindRelationshipAsync(requester.Id, experimentNote.Owner,
                            (byte) StatusRelation.Active);

                // No relationship is found
                if (relationship == null)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
            }

            #endregion

            try
            {
                // Name is defined/
                if (!string.IsNullOrWhiteSpace(modifier.Name))
                    experimentNote.Name = modifier.Name;

                // Information is specified.
                if (modifier.Infos != null)
                    experimentNote.Info = JsonConvert.SerializeObject(modifier.Infos);

                // Update the last modified time.
                experimentNote.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Update the experiment note.
                experimentNote = await _repositoryExperimentNote.InitializeExperimentNote(experimentNote);

                // Send the list of failed record back to client.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    ExperimentNote = new
                    {
                        experimentNote.Id,
                        MedicalRecord = experimentNote.MedicalRecordId,
                        experimentNote.Name,
                        experimentNote.Info,
                        experimentNote.Created,
                        experimentNote.LastModified
                    }
                });
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Delete a medical experiment note or only its key-value pairs.
        /// </summary>
        /// <param name="experiment">Experiment which contains records.</param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> DeleteMedialExperimentNote([FromUri] int experiment)
        {
            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Remove note and retrieve the response.
                var records = await _repositoryExperimentNote.DeleteExperimentNotesAsync(experiment, requester.Id);

                // No record has been removed.
                if (records < 1)
                {
                    // Tell the client that record is not found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Send the list of failed record back to client.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Filter medical by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/experiment/filter")]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterMedicalExperimentNoteAsync(
            [FromBody] FilterExperimentNoteViewModel filter)
        {
            #region Request parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterExperimentNoteViewModel();
                Validate(filter);
            }

            // Request paramters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client.");

                // Tell the client about error.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Medical experiment filter

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Update the filter.
                filter.Requester = requester.Id;

                // Do the filter.
                var result = await _repositoryExperimentNote.FilterExperimentNotesAsync(filter);

                // Tell the client about the filter result.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    ExperimentNotes = result.ExperimentNotes.Select(x => new
                    {
                        x.Id,
                        MedicalRecord = x.MedicalRecordId,
                        x.Owner,
                        x.Creator,
                        x.Name,
                        x.Info,
                        x.Created,
                        x.LastModified
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // As the exception happens, log the error.
                _log.Error(exception.Message, exception);

                // Tell the client about the internal server error.
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
        ///     Repository experiment note
        /// </summary>
        private readonly IRepositoryExperimentNote _repositoryExperimentNote;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Service which provides functions to access calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}