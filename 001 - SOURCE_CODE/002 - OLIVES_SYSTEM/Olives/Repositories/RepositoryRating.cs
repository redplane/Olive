using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Enumerations.Filter;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Repositories
{
    public class RepositoryRating : IRepositoryRating
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructor

        public RepositoryRating(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initialize or update rating asynchronously.
        /// </summary>
        /// <param name="rating"></param>
        /// <param name="rated"></param>
        /// <returns></returns>
        public async Task<Rating> InitializeRatingAsync(Rating rating, int rated)
        {
            var context = _dataContext.Context;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Re-calculate the rating of target person.
                    var ownedRatings = context.Ratings.Where(x => x.Target == rating.Target);
                    var totalRating = await ownedRatings.CountAsync() + 1;
                    double totalMark = rating.Value;
                    foreach (var ownedRating in ownedRatings)
                        totalMark += ownedRating.Value;

                    // Update the rank of doctor.
                    var doctor = context.Doctors.FirstOrDefault(x => x.Id == rated);
                    doctor.Rank = totalMark/totalRating;
                    doctor.Voters = totalRating;
                    context.Doctors.AddOrUpdate(doctor);

                    // Initialize or update to database context.
                    context.Ratings.AddOrUpdate(rating);

                    // Save changes asynchronously.
                    await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return rating;
                }
                catch
                {
                    // Error happens, roll back the transaction and throw the error.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        ///     Filter rates by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseRatingFilter> FilterRatingAsync(FilterRatingViewModel filter)
        {
            // By default, take all records.
            var context = _dataContext.Context;
            IQueryable<Rating> ratings = context.Ratings;

            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                ratings = ratings.Where(x => x.Target == filter.Requester);
                if (filter.Partner != null)
                    ratings = ratings.Where(x => x.Maker == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                ratings = ratings.Where(x => x.Maker == filter.Requester);
                if (filter.Partner != null)
                    ratings = ratings.Where(x => x.Target == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    ratings =
                        ratings.Where(x => x.Maker == filter.Requester || x.Target == filter.Requester);
                else
                    ratings =
                        ratings.Where(
                            x =>
                                (x.Maker == filter.Requester && x.Target == filter.Partner.Value) ||
                                (x.Maker == filter.Partner.Value && x.Target == filter.Requester));
            }


            // Created is defined.
            if (filter.MinCreated != null) ratings = ratings.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null) ratings = ratings.Where(x => x.Created <= filter.MaxCreated.Value);

            // Value is defined.
            if (filter.MinValue != null) ratings = ratings.Where(x => x.Value >= filter.MinValue.Value);
            if (filter.MaxValue != null) ratings = ratings.Where(x => x.Value <= filter.MaxValue.Value);

            // Result sort.
            switch (filter.Direction)
            {
                case SortDirection.Ascending:
                    switch (filter.Sort)
                    {
                        case RatingResultSort.Created:
                            ratings = ratings.OrderBy(x => x.Created);
                            break;
                        default:
                            ratings = ratings.OrderBy(x => x.Value);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case RatingResultSort.Created:
                            ratings = ratings.OrderByDescending(x => x.Created);
                            break;
                        default:
                            ratings = ratings.OrderByDescending(x => x.Value);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseRatingFilter();

            // Calculate the matched results.
            response.Total = await ratings.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                ratings = ratings.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Initialize a list of filtered rate.
            response.Rates = await ratings
                .ToListAsync();

            return response;
        }

        #endregion
    }
}