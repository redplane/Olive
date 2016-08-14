using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Enumerations.Filter;
using Olives.Interfaces.PersonalNote;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;

namespace Olives.Repositories.PersonalNote
{
    public class RepositoryDiary : IRepositoryDiary
    {
        #region Properties

        /// <summary>
        ///     Context which provides functions to access database.
        /// </summary>
        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize an instance of repository with database context setting.
        /// </summary>
        /// <param name="dataContext"></param>
        public RepositoryDiary(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Delete diaries by using conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteDiaryAsync(FilterDiaryViewModel filter)
        {
            var context = _dataContext.Context;
            IQueryable<Diary> diaries = context.Diaries;
            diaries = FilterDiaries(diaries, filter);

            context.Diaries.RemoveRange(diaries);
            return await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Filter diaries by using specific conditions asychronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseDiaryFilter> FilterDiariesAsync(FilterDiaryViewModel filter)
        {
            var context = _dataContext.Context;
            IQueryable<Diary> diaries = context.Diaries;
            diaries = FilterDiaries(diaries, filter);

            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case DiaryFilterSort.Created:
                            diaries = diaries.OrderByDescending(x => x.Created);
                            break;
                        case DiaryFilterSort.LastModified:
                            diaries = diaries.OrderByDescending(x => x.LastModified);
                            break;
                        default:
                            diaries = diaries.OrderByDescending(x => x.Time);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case DiaryFilterSort.Created:
                            diaries = diaries.OrderBy(x => x.Created);
                            break;
                        case DiaryFilterSort.LastModified:
                            diaries = diaries.OrderBy(x => x.LastModified);
                            break;
                        default:
                            diaries = diaries.OrderBy(x => x.Time);
                            break;
                    }
                    break;
            }

            var response = new ResponseDiaryFilter();
            response.Total = await diaries.CountAsync();

            // Pagination is defined
            if (filter.Records != null)
                diaries = diaries.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);

            response.Diaries = diaries;
            return response;
        }

        /// <summary>
        ///     Find a diary by using specific id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Diary> FindDiaryAsync(int id)
        {
            var context = _dataContext.Context;
            return await context.Diaries.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Initialize / update a diary.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<Diary> InitializeDiaryAsync(Diary initializer)
        {
            var context = _dataContext.Context;
            context.Diaries.AddOrUpdate(initializer);

            await context.SaveChangesAsync();
            return initializer;
        }

        private IQueryable<Diary> FilterDiaries(IQueryable<Diary> diaries, FilterDiaryViewModel filter)
        {
            // Id is specified.
            if (filter.Id != null)
                diaries = diaries.Where(x => x.Id == filter.Id);

            // Owner is specified.
            if (filter.Owner != null)
                diaries = diaries.Where(x => x.Owner == filter.Owner);

            if (filter.Target != null)
                diaries = diaries.Where(x => x.Target == filter.Target.Value);

            // Note is specified.
            if (!string.IsNullOrWhiteSpace(filter.Note))
                diaries = diaries.Where(x => x.Note.Contains(filter.Note));

            // Time is specified.
            if (filter.MinTime != null) diaries = diaries.Where(x => x.Time >= filter.MinTime);
            if (filter.MaxTime != null) diaries = diaries.Where(x => x.Time <= filter.MaxTime);

            // Created is specified.
            if (filter.MinCreated != null) diaries = diaries.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) diaries = diaries.Where(x => x.Created <= filter.MaxCreated);

            // Last modified is specified.
            if (filter.MinLastModified != null) diaries = diaries.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null) diaries = diaries.Where(x => x.LastModified <= filter.MaxLastModified);

            return diaries;
        }

        #endregion
    }
}