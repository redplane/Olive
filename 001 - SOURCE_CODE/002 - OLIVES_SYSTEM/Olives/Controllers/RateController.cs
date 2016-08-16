using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    public class RateController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="repositoryRating"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        public RateController(IRepositoryAccountExtended repositoryAccountExtended, IRepositoryRating repositoryRating,
            IRepositoryRelationship repositoryRelation,
            ITimeService timeService, ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositoryRating = repositoryRating;
            _repositoryRelation = repositoryRelation;
            _timeService = timeService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for initializing rating for a doctor.
        ///     Only patient can access this function.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/rating")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> InitializeRatingAsync([FromBody] InitializeRatingViewModel initializer)
        {
            #region Request parameters validation

            // Initializer hasn't been initialized.
            if (initializer == null)
            {
                initializer = new InitializeRatingViewModel();
                Validate(initializer);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Initialization & handling

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Check the relationship between requester and the rated.
                var isRelationshipAvailable =
                    await _repositoryRelation.IsPeopleConnected(requester.Id, initializer.Target);

                // No relationship has been found.
                if (!isRelationshipAvailable)
                {
                    // Log the error.
                    _log.Error(
                        $"There is no relationship between requester [Id: {requester.Id}] and the rated [Id: {initializer.Target}]");

                    // Tell the client about this error.
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }

                #region Rating duplicate check

                var filter = new FilterRatingViewModel();
                filter.Requester = requester.Id;
                filter.Partner = initializer.Target;
                filter.Mode = RecordFilterMode.RequesterIsCreator;

                // Do the filter.
                var result = await _repositoryRating.FilterRatingAsync(filter);
                if (result.Rates != null && result.Rates.Count > 0)
                {
                    // Log the error first.
                    _log.Error($"The rating of requester [Id: {requester.Id}] and rated [Id: {initializer.Target}]");

                    // The rate has been done before.
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        Error = $"{Language.WarnRatingHasBeenDone}"
                    });
                }

                #endregion

                #region Result initialization

                var rating = new Rating();
                rating.Maker = requester.Id;
                rating.Target = initializer.Target;
                rating.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                rating.Value = (byte) initializer.Rate;
                rating.Comment = initializer.Comment;

                // Initialize rating.
                await _repositoryRating.InitializeRatingAsync(rating, initializer.Target);

                return Request.CreateResponse(HttpStatusCode.OK);

                #endregion
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                // Tell the client about the internal server error.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     This function is for filtering rating of requester.
        ///     Patient & doctor can access this function.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/rating/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> FilterRatingAsync([FromBody] FilterRatingViewModel filter)
        {
            #region Paramters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                // Initialize filter and validate model.
                filter = new FilterRatingViewModel();
                Validate(filter);
            }

            // Model state is invalid.
            if (!ModelState.IsValid)
            {
                // Log the error and sent validation errors to client.
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Filter

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Update the filter.
                filter.Requester = requester.Id;

                // Do the filter.
                var result = await _repositoryRating.FilterRatingAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Ratings = result.Rates.Select(x => new
                    {
                        Maker = new
                        {
                            Id = x.Maker,
                            x.Patient.Person.FirstName,
                            x.Patient.Person.LastName
                        },
                        Target = new
                        {
                            Id = x.Target,
                            x.Doctor.Person.FirstName,
                            x.Doctor.Person.LastName
                        },
                        x.Value,
                        x.Comment,
                        x.Created,
                        x.LastModified
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                // Tell the client about the error.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Repository of rating.
        /// </summary>
        private readonly IRepositoryRating _repositoryRating;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelationship _repositoryRelation;

        /// <summary>
        ///     Service which provides functions to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}