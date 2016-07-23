﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Models;
using Olives.ViewModels.Initialize;
using Olives.ViewModels.Modify;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Olives.Controllers
{
    [Route("api/medical/experiment")]
    public class MedicalExperimentController : ApiParentController
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
        public MedicalExperimentController(IRepositoryAccount repositoryAccount, IRepositoryMedical repositoryMedical,
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

            #endregion

            #region Relationship validation

            // Requester is different from the medical owner.
            if (requester.Id != medicalRecord.Owner)
            {
                // Find the relationship between the requester and prescription owner.
                var relationship =
                    await
                        _repositoryAccount.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
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
                note.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
                note.MedicalRecordId = initializer.MedicalRecord;
                note.Name = initializer.Name;
                note.Owner = medicalRecord.Owner;
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
            var experimentNote = await _repositoryMedical.FindExperimentNoteAsync(experiment);

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is different from the medical owner.
            if (requester.Id != experimentNote.Owner)
            {
                // Find the relationship between the requester and prescription owner.
                var relationship =
                    await
                        _repositoryAccount.FindRelationshipAsync(requester.Id, experimentNote.Owner,
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
                experimentNote.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

                // Update the experiment note.
                experimentNote = await _repositoryMedical.InitializeExperimentNote(experimentNote);

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
                var records = await _repositoryMedical.DeleteExperimentNotesAsync(experiment, requester.Id);

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