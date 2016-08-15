using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.Interfaces.PersonalNote;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Olives.Controllers
{
    [Route("api/diary")]
    [OlivesAuthorize(new[] {Role.Doctor})]
    public class DiaryController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of DiaryController with Dependency injections.
        /// </summary>
        /// <param name="repositoryDiary"></param>
        /// <param name="repositoryRelationship"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        public DiaryController(
            IRepositoryDiary repositoryDiary, IRepositoryRelationship repositoryRelationship,
            ITimeService timeService,
            ILog log)
        {
            _repositoryDiary = repositoryDiary;
            _repositoryRelationship = repositoryRelationship;
            _timeService = timeService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a diary by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> FindDiaryAsync([FromUri] int id)
        {
            try
            {
                #region Record find

                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Find the medical record by using id.
                var diary = await _repositoryDiary.FindDiaryAsync(id);

                // No result has been received.
                if (diary == null)
                {
                    // Log the error.
                    _log.Error($"Diary [Id: {id}] is not found");

                    // Tell client no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                #endregion

                #region Owner check

                if (diary.Owner != requester.Id)
                {
                    _log.Error($"Requester [Id: {requester.Id}] is not the owner of diary [Id:{id}]");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Diary = new
                    {
                        diary.Id,
                        diary.Owner,
                        diary.Target,
                        diary.Note,
                        diary.Time,
                        diary.Created,
                        diary.LastModified
                    }
                });

                #endregion
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
        ///     Add a diary asyncrhonously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> InitializeDiaryAsync(
            [FromBody] InitializeDiaryViewModel initializer)
        {
            #region Parameters validation

            // Model hasn't been initialized.
            if (initializer == null)
            {
                // Initialize it and do the validation.
                initializer = new InitializeDiaryViewModel();
                Validate(initializer);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Relationship validation

            // Find the requester.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the relationship.
            var rPeopleConnected = await _repositoryRelationship.IsPeopleConnected(requester.Id, initializer.Target);
            if (!rPeopleConnected)
            {
                _log.Error($"There is no relationship between requester [Id: {requester.Id}] and Target [Id: {initializer.Target}]");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
                });
            }

            #endregion

            #region Result handling

            try
            {
                #region Result initialization
                
                // Initialize an instance of MedicalNote.
                var diary = new Diary();
                diary.Note = initializer.Note;
                diary.Owner = requester.Id;
                diary.Target = initializer.Target;
                diary.Time = initializer.Time;
                diary.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert a new allergy to database.
                diary = await _repositoryDiary.InitializeDiaryAsync(diary);

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Diary = new
                    {
                        diary.Id,
                        diary.Note,
                        diary.Time,
                        diary.Target,
                        diary.Owner,
                        diary.Created
                    }
                });

                #endregion
            }
            catch (Exception exception)
            {
                // As the exception happens, log the error first.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Add a medical record asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="edit"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<HttpResponseMessage> EditDiaryAsync([FromUri] int id,
            [FromBody] EditDiaryViewModel edit)
        {
            #region Parameters validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Model hasn't been initialized.
            if (edit == null)
            {
                // Initialize it and do the validation.
                edit = new EditDiaryViewModel();
                Validate(edit);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result find

            // Find the medical note.
            var diary = await _repositoryDiary.FindDiaryAsync(id);

            // Medical note is not found.
            if (diary == null)
            {
                _log.Error($"Diary [Id: {id}] is not found");
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Owner check

            // No relationship is found.
            if (requester.Id != diary.Owner)
            {
                // Log the error.
                _log.Error(
                    $"Requester [Id: {requester.Id}] is not the creator of diary [Id: {diary.Id}]");
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnRequesterNotInRecord}"
                });
            }

            #endregion

            #region Information update

            try
            {
                #region Result update

                var recordChanged = false;

                if (!string.IsNullOrWhiteSpace(edit.Note))
                {
                    diary.Note = edit.Note;
                    recordChanged = true;
                }

                if (edit.Time != null)
                {
                    diary.Time = edit.Time.Value;
                    recordChanged = true;
                }

                if (recordChanged)
                {
                    diary.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                    diary = await _repositoryDiary.InitializeDiaryAsync(diary);
                }

                #endregion

                #region Result handling

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Diary = new
                    {
                        diary.Id,
                        diary.Owner,
                        diary.Target,
                        diary.Note,
                        diary.Time,
                        diary.Created,
                        diary.LastModified
                    }
                });

                #endregion
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
        ///     Delete a diary asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteDiaryAsync([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Filter initialization.
            var filter = new FilterDiaryViewModel();
            filter.Owner = requester.Id;
            filter.Id = id;

            try
            {
                var records = await _repositoryDiary.DeleteDiaryAsync(filter);
                if (records < 1)
                {
                    _log.Error($"No diary [Id: {id}] owned by person [Id: {requester.Id}] is found");
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/diary/filter")]
        [HttpPost]
        public async Task<HttpResponseMessage> FilterMedicalNote([FromBody] FilterDiaryViewModel filter)
        {
            #region Parameters validation

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Model hasn't been initialized.
            if (filter == null)
            {
                // Initialize it and do the validation.
                filter = new FilterDiaryViewModel();
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
                // Update owner of filter. Doctor can only see his/her records.
                filter.Owner = requester.Id;

                // Insert a new allergy to database.
                var result = await _repositoryDiary.FilterDiariesAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Diaries = result.Diaries.Select(x => new
                    {
                        x.Id,
                        x.Note,
                        x.Owner,
                        x.Target,
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
        ///     Repository which provides functions to access diary database.
        /// </summary>
        private readonly IRepositoryDiary _repositoryDiary;

        
        private readonly IRepositoryRelationship _repositoryRelationship;

        /// <summary>
        ///     Service which provides function to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}