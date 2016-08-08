using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces.PersonalNote;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Repositories.PersonalNote
{
    public class RepositoryBloodPressure : IRepositoryBloodPressure
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryBloodPressure(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        /// <summary>
        ///     Initialize a blood pressure note to database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<BloodPressure> InitializeBloodPressureNoteAsync(BloodPressure info)
        {
            // Add allergy to database context.
            var context = _dataContext.Context;
            context.BloodPressures.AddOrUpdate(info);

            // Submit allergy.
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        ///     Find heartbeat note by using id and owner id.
        /// </summary>
        /// <param name="id">Allergy Id</param>
        /// <returns></returns>
        public async Task<BloodPressure> FindBloodPressureNoteAsync(int id)
        {
            // Find heartbeat note by using id.
            var context = _dataContext.Context;
            return await context.BloodPressures.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Delete a blood pressure note asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteBloodPressureNoteAsync(FilterBloodPressureViewModel filter)
        {
            #region Record filter

            // By default, take all information.
            var context = _dataContext.Context;
            IQueryable<BloodPressure> bloodPressures = context.BloodPressures;
            bloodPressures = FilterBloodPressuresAsync(bloodPressures, filter);

            #endregion

            #region Record handling

            // Find and delete a note.
            context.BloodPressures.RemoveRange(bloodPressures);

            // Count the affeted records.
            var records = await context.SaveChangesAsync();
            return records;

            #endregion
        }

        /// <summary>
        ///     Find heartbeat by using conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseBloodPressureFilter> FilterBloodPressureNoteAsync(FilterBloodPressureViewModel filter)
        {
            // By default, take all information.
            var context = _dataContext.Context;
            IQueryable<BloodPressure> bloodPressures = context.BloodPressures;
            bloodPressures = FilterBloodPressuresAsync(bloodPressures, filter);

            // Result sorting.
            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            bloodPressures = bloodPressures.OrderByDescending(x => x.Created);
                            break;
                        case NoteResultSort.LastModified:
                            bloodPressures = bloodPressures.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            bloodPressures = bloodPressures.OrderByDescending(x => x.Time);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            bloodPressures = bloodPressures.OrderBy(x => x.Created);
                            break;
                        case NoteResultSort.LastModified:
                            bloodPressures = bloodPressures.OrderBy(x => x.LastModified);
                            break;
                        default:
                            bloodPressures = bloodPressures.OrderBy(x => x.Time);
                            break;
                    }
                    break;
            }

            // Initialize response and throw result back.
            var response = new ResponseBloodPressureFilter();
            response.Total = await bloodPressures.CountAsync();

            // Record is defined.
            if (filter.Records != null)
            {
                bloodPressures = bloodPressures.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Response results construction.
            response.BloodPressures = bloodPressures;

            // Return filtered result.
            return response;
        }

        /// <summary>
        ///     Filter blood pressure notes by using specific conditions.
        /// </summary>
        /// <param name="bloodPressures"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<BloodPressure> FilterBloodPressuresAsync(IQueryable<BloodPressure> bloodPressures,
            FilterBloodPressureViewModel filter)
        {
            // Id is specified.
            if (filter.Id != null)
                bloodPressures = bloodPressures.Where(x => x.Id == filter.Id.Value);

            // Owner has been specified.
            if (filter.Owner != null)
                bloodPressures = bloodPressures.Where(x => x.Owner == filter.Owner);

            // Systolic has been specified.
            if (filter.MinSystolic != null)
                bloodPressures = bloodPressures.Where(x => x.Systolic >= filter.MinSystolic);
            if (filter.MaxSystolic != null)
                bloodPressures = bloodPressures.Where(x => x.Systolic <= filter.MaxSystolic);

            // Diastolic has been specified.
            if (filter.MinDiastolic != null)
                bloodPressures = bloodPressures.Where(x => x.Diastolic >= filter.MinDiastolic);
            if (filter.MaxDiastolic != null)
                bloodPressures = bloodPressures.Where(x => x.Diastolic <= filter.MaxDiastolic);


            // Time has been specified.
            if (filter.MinTime != null)
                bloodPressures = bloodPressures.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null)
                bloodPressures = bloodPressures.Where(x => x.Time <= filter.MaxTime);

            // Created has been specified.
            if (filter.MinCreated != null)
                bloodPressures = bloodPressures.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                bloodPressures = bloodPressures.Where(x => x.Created <= filter.MaxCreated);

            // LastModified has been specified.
            if (filter.MinLastModified != null)
                bloodPressures = bloodPressures.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                bloodPressures = bloodPressures.Where(x => x.LastModified <= filter.MaxLastModified);

            // Note has been specified.
            if (!string.IsNullOrEmpty(filter.Note))
                bloodPressures = bloodPressures.Where(x => x.Note.Contains(filter.Note));

            return bloodPressures;
        }
    }
}