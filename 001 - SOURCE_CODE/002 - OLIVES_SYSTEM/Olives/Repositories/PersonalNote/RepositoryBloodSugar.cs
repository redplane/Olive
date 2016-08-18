using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Enumerations.Filter;
using Olives.Interfaces.PersonalNote;
using Olives.ViewModels.Filter.Personal;
using Olives.ViewModels.Response.Personal;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Repositories.PersonalNote
{
    public class RepositoryBloodSugar : IRepositoryBloodSugar
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryBloodSugar(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initialize sugarblood note to database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<SugarBlood> InitializeBloodSugarAsync(SugarBlood info)
        {
            // Add allergy to database context.
            var context = _dataContext.Context;

            context.SugarBloods.AddOrUpdate(info);

            // Submit allergy.
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        ///     Find sugarblood note by using id and owner id.
        /// </summary>
        /// <param name="id">Blood sugar Id</param>
        /// <returns></returns>
        public async Task<SugarBlood> FindBloodSugarAsync(int id)
        {
            var context = _dataContext.Context;
            return await context.SugarBloods.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Find blood sugar notes by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseBloodSugarFilter> FilterBloodSugarAsync(FilterBloodSugarViewModel filter)
        {
            // By default, take all information.
            var context = _dataContext.Context;
            IQueryable<SugarBlood> bloodSugars = context.SugarBloods;
            bloodSugars = FilterBloodSugars(bloodSugars, filter);

            // Initialize response and throw result back.
            var response = new ResponseBloodSugarFilter();
            response.Total = await bloodSugars.CountAsync();

            // Sort the result.
            switch (filter.Sort)
            {
                case NoteResultSort.Created:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        bloodSugars = bloodSugars.OrderBy(x => x.Created);
                        break;
                    }

                    bloodSugars = bloodSugars.OrderByDescending(x => x.Created);
                    break;
                case NoteResultSort.LastModified:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        bloodSugars = bloodSugars.OrderBy(x => x.LastModified);
                        break;
                    }
                    bloodSugars = bloodSugars.OrderByDescending(x => x.LastModified);
                    break;
                default:
                    if (filter.Direction == SortDirection.Ascending)
                    {
                        bloodSugars = bloodSugars.OrderBy(x => x.Time);
                        break;
                    }

                    bloodSugars = bloodSugars.OrderByDescending(x => x.Time);
                    break;
            }

            // Record is defined.
            if (filter.Records != null)
            {
                bloodSugars = bloodSugars.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            response.Sugarbloods = bloodSugars;

            // Return filtered result.
            return response;
        }

        /// <summary>
        ///     Filter blood sugar notes by using specific conditions.
        /// </summary>
        /// <param name="bloodSugars"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<SugarBlood> FilterBloodSugars(IQueryable<SugarBlood> bloodSugars,
            FilterBloodSugarViewModel filter)
        {
            // Id is specified.
            if (filter.Id != null)
                bloodSugars = bloodSugars.Where(x => x.Id == filter.Id);

            // Owner has been specified.
            if (filter.Owner != null)
                bloodSugars = bloodSugars.Where(x => x.Owner == filter.Owner);

            // Value has been specified.
            if (filter.MinValue != null)
                bloodSugars = bloodSugars.Where(x => x.Value >= filter.MinValue);
            if (filter.MinValue != null)
                bloodSugars = bloodSugars.Where(x => x.Value <= filter.MaxValue);

            // Time has been specified.
            if (filter.MinTime != null)
                bloodSugars = bloodSugars.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null)
                bloodSugars = bloodSugars.Where(x => x.Time <= filter.MaxTime);

            // Created has been specified.
            if (filter.MinCreated != null)
                bloodSugars = bloodSugars.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                bloodSugars = bloodSugars.Where(x => x.Created <= filter.MaxCreated);

            // LastModified has been specified.
            if (filter.MinLastModified != null)
                bloodSugars = bloodSugars.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                bloodSugars = bloodSugars.Where(x => x.LastModified <= filter.MaxLastModified);

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                bloodSugars = bloodSugars.Where(x => x.Note.Contains(filter.Note));

            // Order by last modified.
            bloodSugars = bloodSugars.OrderByDescending(x => x.LastModified);
            return bloodSugars;
        }

        /// <summary>
        ///     Delete a sugarblood note asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        public async Task<int> DeleteBloodSugarAsync(FilterBloodSugarViewModel filter)
        {
            // By default, take all information.
            var context = _dataContext.Context;
            IQueryable<SugarBlood> bloodSugars = context.SugarBloods;
            bloodSugars = FilterBloodSugars(bloodSugars, filter);

            // Delete the filtered record.
            context.SugarBloods.RemoveRange(bloodSugars);

            // Save changes and count the affected records.
            var records = await context.SaveChangesAsync();
            return records;
        }

        #endregion
    }
}