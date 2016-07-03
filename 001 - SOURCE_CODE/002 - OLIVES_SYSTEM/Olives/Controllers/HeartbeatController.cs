﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Initialize;

namespace Olives.Controllers
{
    [Route("api/heartbeat")]
    [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
    public class HeartbeatController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryHeartbeat"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        public HeartbeatController(IRepositoryAccount repositoryAccount, IRepositoryHeartbeat repositoryHeartbeat,
            ILog log, IEmailService emailService)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryHeartbeat = repositoryHeartbeat;
            _log = log;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Retrieve the results list.
            var results = await _repositoryHeartbeat.FindHeartbeatAsync(id, requester.Id);

            // No result has been received.
            if (results == null || results.Count != 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }
            // Retrieve the 1st queried result.
            var result = results
                .Select(x => new HeartbeatViewModel
                {
                    Id = x.Id,
                    Created = x.Created,
                    LastModified = x.LastModified,
                    Time = x.Time,
                    Note = x.Note,
                    Rate = x.Rate
                })
                .FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeat = result
            });
        }

        /// <summary>
        ///     Insert an allergy asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeHeartbeatViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                info = new InitializeHeartbeatViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var heartbeat = new Heartbeat();
            heartbeat.Owner = requester.Id;
            heartbeat.Rate = info.Rate;
            heartbeat.Note = info.Note;
            heartbeat.Time = info.Time;
            heartbeat.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryHeartbeat.InitializeHeartbeatNoteAsync(heartbeat);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeat = new
                {
                    result.Id,
                    result.Rate,
                    result.Time,
                    result.Note,
                    result.Created
                }
            });
        }

        /// <summary>
        ///     Edit an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] InitializeHeartbeatViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                info = new InitializeHeartbeatViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var results = await _repositoryHeartbeat.FindHeartbeatAsync(id, requester.Id);

            // Not record has been found.
            if (results == null || results.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Records are conflict.
            if (results.Count != 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first record.
            var result = results.FirstOrDefault();
            if (result == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Confirm edit.
            result.Rate = info.Rate;
            result.Time = info.Time;
            result.Note = info.Note;

            // Update allergy.
            result = await _repositoryHeartbeat.InitializeHeartbeatNoteAsync(result);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Heartbeat = new HeartbeatViewModel
                {
                    Id = result.Id,
                    Time = result.Time,
                    Created = result.Created,
                    LastModified = result.LastModified,
                    Note = result.Note,
                    Rate = result.Rate
                }
            });
        }

        /// <summary>
        ///     Delete an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var result = await _repositoryHeartbeat.FindHeartbeatAsync(id, requester.Id);

            // Not record has been found.
            if (result == null || result.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Records are conflict.
            if (result.Count != 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first record.
            var heartbeat = result.FirstOrDefault();
            if (heartbeat == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Remove the found allergy.
            _repositoryHeartbeat.DeleteHeartbeatNoteAsync(heartbeat);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/heartbeat/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterHeatbeatViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                info = new FilterHeatbeatViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Person can only see his/her notes.
            info.Owner = requester.Id;

            // Retrieve the results list.
            var results = await _repositoryHeartbeat.FilterHeartbeatAsync(info);

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of heartbeats
        /// </summary>
        private readonly IRepositoryHeartbeat _repositoryHeartbeat;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used for sending emails.
        /// </summary>
        private readonly IEmailService _emailService;

        #endregion
    }
}