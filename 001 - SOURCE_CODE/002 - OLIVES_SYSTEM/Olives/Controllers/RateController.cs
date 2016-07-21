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
        public async Task<HttpResponseMessage> InitializeRating(InitializeRatingViewModel initializer)
        {
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

            // Find the rated person.
            var rated = await _repositoryAccount.FindPersonAsync(initializer.Target, null, null, (byte) Role.Doctor,
                StatusAccount.Active);

            // The rated isn't found.
            if (rated == null)
            {
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
            if (relationships.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
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

            // Initialize rating.
            await _repositoryRating.InitializeRatingAsync(rating, rated);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     This function is for filtering rating of requester.
        ///     Patient & doctor can access this function.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/rating/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> FilterRating(FilterRatingViewModel filter)
        {
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

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Requester is a patient, set id to maker.
            if (requester.Role == (byte) Role.Patient)
                filter.Maker = requester.Id;
            else
                filter.Target = requester.Id;

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
                    x.Comment,
                    x.Created,
                    x.LastModified
                }),
                result.Total
            });
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