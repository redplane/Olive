﻿using System.Data.Entity;
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
    public class RepositoryAddiction : IRepositoryAddiction
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryAddiction(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Delete an addiction asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> DeleteAddictionAsync(FilterAddictionViewModel filter)
        {
            var context = _dataContext.Context;
            IQueryable<Addiction> addictions = context.Addictions;
            addictions = FilterAddictionsAsync(addictions, filter);

            // Delete and retrieve the affected records.
            context.Addictions.RemoveRange(addictions);

            // Save changes.
            var records = await context.SaveChangesAsync();

            return records;
        }

        /// <summary>
        ///     Filter a list of addiction with the specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseAddictionFilter> FilterAddictionsAsync(FilterAddictionViewModel filter)
        {
            // By default, take all records from database.
            var context = _dataContext.Context;
            IQueryable<Addiction> addictions = context.Addictions;
            addictions = FilterAddictionsAsync(addictions, filter);

            // Result filter
            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            addictions = addictions.OrderByDescending(x => x.Created);
                            break;
                        default:
                            addictions = addictions.OrderByDescending(x => x.LastModified);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case NoteResultSort.Created:
                            addictions = addictions.OrderBy(x => x.Created);
                            break;
                        default:
                            addictions = addictions.OrderBy(x => x.LastModified);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseAddictionFilter();

            // Count the number of matched records.
            response.Total = await addictions.CountAsync();

            // Result pagination.
            if (filter.Records != null)
            {
                addictions = addictions.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Take the paginated results.
            response.Addictions = addictions;

            return response;
        }

        /// <summary>
        ///     Filter the addictions by using specific conditions.
        /// </summary>
        /// <param name="addictions"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<Addiction> FilterAddictionsAsync(IQueryable<Addiction> addictions,
            FilterAddictionViewModel filter)
        {
            // Id is specified.
            if (filter.Id != null)
                addictions = addictions.Where(x => x.Id == filter.Id);

            // Owner has been specified.
            if (filter.Owner != null)
                addictions = addictions.Where(x => x.Owner == filter.Owner);

            // Cause has been specified.
            if (!string.IsNullOrWhiteSpace(filter.Cause))
                addictions = addictions.Where(x => x.Cause.Contains(filter.Cause));

            // Note has been specified.
            if (!string.IsNullOrWhiteSpace(filter.Note))
                addictions = addictions.Where(x => x.Note.Contains(filter.Note));

            // Created has been specified.
            if (filter.MinCreated != null) addictions = addictions.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null) addictions = addictions.Where(x => x.Created <= filter.MaxCreated);

            // Last modified has been specified.
            if (filter.MinLastModified != null)
                addictions = addictions.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                addictions = addictions.Where(x => x.LastModified <= filter.MaxLastModified);

            return addictions;
        }

        /// <summary>
        ///     Find an addiction by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Addiction> FindAddictionAsync(int id)
        {
            // Find the first matched addiction with id.
            var context = _dataContext.Context;
            var result = await context.Addictions.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        /// <summary>
        ///     Initialize an addiction asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Addiction> InitializeAddictionAsync(Addiction info)
        {
            // Insert the source record to database.
            var context = _dataContext.Context;

            context.Addictions.AddOrUpdate(info);
            await context.SaveChangesAsync();
            return info;
        }

        #endregion
    }
}