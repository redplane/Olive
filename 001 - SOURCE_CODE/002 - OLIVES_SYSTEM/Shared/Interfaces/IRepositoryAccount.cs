﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        #region Shared

        /// <summary>
        ///     Find person by using specific information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IList<Person>> LoginAsync(LoginViewModel info);

        /// <summary>
        ///     Find person by using specific information synchronously.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email of person</param>
        /// <param name="password">Password of person</param>
        /// <param name="role"></param>
        /// <returns></returns>
        Person FindPerson(int? id, string email, string password, byte? role);

        /// <summary>
        ///     Find person by using specific information asynchronously.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email of person</param>
        /// <param name="password">Password of person</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Person> FindPersonAsync(int? id, string email, string password, byte? role, StatusAccount? status);

        /// <summary>
        /// Find a person asynchronously by using activation code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Person> FindPersonAsync(string code);

        /// <summary>
        ///     Edit person status asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Person> EditPersonStatusAsync(int id, byte status);

        /// <summary>
        ///     Summary person by using role.
        /// </summary>
        /// <returns></returns>
        Task<IList<StatusSummaryViewModel>> SummarizePersonRole(byte? role);
        
        #endregion

        #region Doctor

        /// <summary>
        ///     Filter doctor by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<DoctorViewModel>> FindDoctorAsync(int id);

        /// <summary>
        ///     Filter doctor by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseDoctorFilter> FilterDoctorAsync(FilterDoctorViewModel filter);

        /// <summary>
        /// Initialize a doctor to database.
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        Task<Doctor> InitializeDoctorAsync(Doctor doctor);

        #endregion

        #region Patient

        /// <summary>
        ///     Find patient by using specific asynchrounously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<PatientViewModel>> FindPatientAsync(int id);

        /// <summary>
        ///     Filter doctor by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter);

        /// <summary>
        /// Initialize a patient to database.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        Task<Patient> InitializePatientAsync(Patient patient);

        /// <summary>
        /// Activate patient's account by search person id.
        /// </summary>
        /// <param name="code"></param>
        Task<bool> ActivatePatientAccount(string code);

        #endregion
    }
}