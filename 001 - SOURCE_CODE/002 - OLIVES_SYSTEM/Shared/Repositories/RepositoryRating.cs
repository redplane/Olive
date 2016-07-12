using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryRating : IRepositoryRating
    {
        /// <summary>
        ///     Initialize or update rating asynchronously.
        /// </summary>
        /// <param name="rating"></param>
        /// <param name="rated"></param>
        /// <returns></returns>
        public async Task<Rating> InitializeRatingAsync(Rating rating, Person rated)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

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
                    rated.Doctor.Rank = totalMark/totalRating;
                    rated.Doctor.Voters = totalRating;
                    context.People.AddOrUpdate(rated);

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
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all records.
            IQueryable<Rating> ratings = context.Ratings;

            // Maker is defined.
            if (filter.Maker != null)
                ratings = ratings.Where(x => x.Maker == filter.Maker.Value);

            // Target is defined.
            if (filter.Target != null)
                ratings = ratings.Where(x => x.Target == filter.Target.Value);

            // Created is defined.
            if (filter.MinCreated != null) ratings = ratings.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null) ratings = ratings.Where(x => x.Created <= filter.MaxCreated.Value);

            // Last modified is defined.
            if (filter.MinLastModified != null) ratings = ratings.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null) ratings = ratings.Where(x => x.LastModified <= filter.MaxLastModified);

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
                        case RatingResultSort.LastModified:
                            ratings = ratings.OrderBy(x => x.LastModified);
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
                        case RatingResultSort.LastModified:
                            ratings = ratings.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            ratings = ratings.OrderByDescending(x => x.Value);
                            break;
                    }
                    break;
            }

            // Calculate the number of records should be skipped.
            var skippedRecords = filter.Page*filter.Records;

            // Response initialization.
            var response = new ResponseRatingFilter();

            // Calculate the matched results.
            response.Total = await ratings.CountAsync();

            // Initialize a list of filtered rate.
            response.Rates = await ratings.Skip(skippedRecords)
                .Take(filter.Records)
                .ToListAsync();

            return response;
        }
    }
}