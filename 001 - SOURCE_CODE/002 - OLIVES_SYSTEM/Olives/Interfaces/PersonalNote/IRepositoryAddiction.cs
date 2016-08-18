﻿using System.Threading.Tasks;
using Olives.ViewModels.Filter.Personal;
using Olives.ViewModels.Response;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.PersonalNote
{
    public interface IRepositoryAddiction
    {
        /// <summary>
        ///     Filter allergies with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseAddictionFilter> FilterAddictionAsync(FilterAddictionViewModel filter);

        /// <summary>
        ///     Initialize an allergy with given information.
        /// </summary>
        /// <param name="info"></param>
        Task<Addiction> InitializeAddictionAsync(Addiction info);

        /// <summary>
        ///     Find an addiction by using id.
        /// </summary>
        /// <param name="id">Addiction Id</param>
        /// <returns></returns>
        Task<Addiction> FindAddictionAsync(int id);

        /// <summary>
        ///     Delete an allergy asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeleteAddictionAsync(FilterAddictionViewModel filter);
    }
}