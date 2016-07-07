using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
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
    public class AddictionController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryAddiction"></param>
        /// <param name="log"></param>
        public AddictionController(IRepositoryAccount repositoryAccount, IRepositoryAddiction repositoryAddiction,
            ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryAddiction = repositoryAddiction;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/addiction")]
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var filter = new FilterAddictionViewModel();
            filter.Id = id;
            filter.Page = 0;
            filter.Records = 1;

            // Retrieve the results list.
            var results = await _repositoryAddiction.FilterAddictionAsync(filter);

            // No result has been received.
            if (results == null || results.Addictions == null || results.Addictions.Count != 1)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnNoRecord}"
                });
            }

            // Retrieve the first queried result.
            var result = results.Addictions.FirstOrDefault();
            if (result == null)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnNoRecord}"
                });
            }

            #region Relation validation

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

            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Addiction = new
                {
                    result.Id,
                    result.Cause,
                    result.Created,
                    result.LastModified,
                    result.Note,
                    result.Owner
                }
            });
        }

        /// <summary>
        ///     Add an addiction asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/addiction")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeAddictionViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new InitializeAddictionViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid addiction request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var addiction = new Addiction();
            addiction.Owner = requester.Id;
            addiction.Cause = info.Cause;
            addiction.Note = info.Note;
            addiction.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryAddiction.InitializeAddictionAsync(addiction);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Addiction = new
                {
                    result.Id,
                    result.Cause,
                    result.Note,
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
        [Route("api/addiction")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Put([FromUri] int id, InitializeAddictionViewModel info)
        {
            // Request parameters haven't been initialized.
            if (info == null)
            {
                info = new InitializeAddictionViewModel();
                Validate(info);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the record first.
            var filter = new FilterAddictionViewModel();
            filter.Id = id;
            filter.Owner = requester.Id;

            // Find the addiction of the requester with the same id.
            var filteredResult = await _repositoryAddiction.FilterAddictionAsync(filter);

            // Invalid record or record is not unique.
            if (filteredResult == null || filteredResult.Addictions == null || filteredResult.Addictions.Count != 1)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnNoRecord}"
                });
            }

            // Retrieve the first queried result.
            var addiction = filteredResult.Addictions.FirstOrDefault();
            if (addiction == null)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnNoRecord}"
                });
            }

            // Update the information.
            addiction.Cause = info.Cause;
            addiction.Note = info.Note;
            addiction.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Update record to database.
            await _repositoryAddiction.InitializeAddictionAsync(addiction);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Addiction = new
                {
                    addiction.Id,
                    addiction.Cause,
                    addiction.Created,
                    addiction.LastModified,
                    addiction.Note,
                    addiction.Owner
                }
            });
        }

        /// <summary>
        ///     Delete an addiction asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/addiction")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Delete([FromBody] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Remove the addiction of the requester.
                var affectedRecords = await _repositoryAddiction.DeleteAddictionAsync(id, requester.Id);

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

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of allergies
        /// </summary>
        private readonly IRepositoryAddiction _repositoryAddiction;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}