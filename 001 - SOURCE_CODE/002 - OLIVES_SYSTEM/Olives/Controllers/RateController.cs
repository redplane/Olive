using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Helpers;
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
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryRating"></param>
        /// <param name="log"></param>
        public RateController(IRepositoryAccount repositoryAccount, IRepositoryRating repositoryRating, ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryRating = repositoryRating;
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

            #region Initialization

            try
            {
                // Find the rated person.
                var rated = await _repositoryAccount.FindPersonAsync(initializer.Target, null, null, (byte) Role.Doctor,
                    StatusAccount.Active);

                // The rated isn't found.
                if (rated == null)
                {
                    // Log the error.
                    _log.Error($"Cannot find the person [Id: {initializer.Rate}]");

                    // Tell the client about the rate.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnTheRatedNotFound}"
                    });
                }

                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Check the relationship between requester and the rated.
                var relationships = await _repositoryAccount.FindRelationshipAsync(requester.Id, rated.Id,
                    (byte) StatusRelation.Active);

                // No relationship has been found.
                if (relationships == null || relationships.Count < 1)
                {
                    // Log the error.
                    _log.Error(
                        $"There is no relationship between requester [Id: {requester.Id}] and the rated [Id: {rated.Id}]");

                    // Tell the client about this error.
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
                }

                // Find the rating.
                var filter = new FilterRatingViewModel();
                filter.Requester = requester.Id;
                filter.Partner = rated.Id;
                filter.Mode = RecordFilterMode.RequesterIsCreator;

                // Do the filter.
                var result = await _repositoryRating.FilterRatingAsync(filter);
                if (result.Rates != null && result.Rates.Count > 0)
                {
                    // Log the error first.
                    _log.Error($"The rating of requester [Id: {requester.Id}] and rated [Id: {rated.Id}]");

                    // The rate has been done before.
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        Error = $"{Language.WarnRatingHasBeenDone}"
                    });
                }

                var rating = new Rating();
                rating.Maker = requester.Id;
                rating.MakerFirstName = requester.FirstName;
                rating.MakerLastName = requester.LastName;
                rating.Target = rated.Id;
                rating.TargetFirstName = rated.FirstName;
                rating.TargetLastName = rated.LastName;
                rating.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
                rating.Value = (byte) initializer.Rate;
                rating.Comment = initializer.Comment;

                // Initialize rating.
                await _repositoryRating.InitializeRatingAsync(rating, rated.Id);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Maker = new
                    {
                        Id = rating.Maker,
                        FirstName = rating.MakerFirstName,
                        LastName = rating.MakerLastName
                    },
                    Target = new
                    {
                        Id = rating.Target,
                        FirstName = rating.TargetFirstName,
                        LastName = rating.TargetLastName
                    },
                    rating.Value,
                    rating.Comment,
                    rating.Created,
                    rating.LastModified
                });
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
                            FirstName = x.MakerFirstName,
                            LastName = x.MakerLastName
                        },
                        Target = new
                        {
                            Id = x.Target,
                            FirstName = x.TargetFirstName,
                            LastName = x.TargetLastName
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
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of rating.
        /// </summary>
        private readonly IRepositoryRating _repositoryRating;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}