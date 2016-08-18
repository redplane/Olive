﻿using System.Threading.Tasks;
using Olives.ViewModels.Filter.Medical;
using Olives.ViewModels.Response.Medical;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Olives.Interfaces.Medical
{
    public interface IRepositoryPrescription
    {
        /// <summary>
        ///     Find the prescription by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Prescription> FindPrescriptionAsync(int id);

        /// <summary>
        ///     Initialize or update an prescription.
        /// </summary>
        /// <param name="prescription"></param>
        /// <returns></returns>
        Task<Prescription> InitializePrescriptionAsync(Prescription prescription);

        /// <summary>
        ///     Delete prescription by using id.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<int> DeletePrescriptionAsync(FilterPrescriptionViewModel filter);

        /// <summary>
        ///     Filter prescription asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePrescriptionFilterViewModel> FilterPrescriptionAsync(
            FilterPrescriptionViewModel filter);
    }
}