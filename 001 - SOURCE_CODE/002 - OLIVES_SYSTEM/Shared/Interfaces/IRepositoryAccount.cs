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
        /// <param name="status"></param>
        /// <returns></returns>
        Person FindPerson(int? id, string email, string password, byte? role, StatusAccount? status);

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
        ///     Find a person asynchronously by using activation code.
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
        Task<IList<StatusSummaryViewModel>> SummarizePersonRoleAsync(byte? role);

        /// <summary>
        ///     Initialize or update person information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Person> InitializePersonAsync(Person info);

        /// <summary>
        ///     Edit a person by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Person> EditPersonProfileAsync(int id, Person info);

        #endregion

        #region Doctor

        /// <summary>
        ///     Filter doctor by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Doctor> FindDoctorAsync(int id, StatusAccount? status);

        /// <summary>
        ///     Filter doctor by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseDoctorFilter> FilterDoctorAsync(FilterDoctorViewModel filter);

        #endregion

        #region Patient

        /// <summary>
        ///     Find the patient by using id and perhaps status asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Patient> FindPatientAsync(int id, byte? status);

        /// <summary>
        ///     Filter patient by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter,
            Person requester = null);

        /// <summary>
        ///     Activate patient's account by search person id.
        /// </summary>
        /// <param name="code"></param>
        Task<bool> InitializePatientActivation(string code);

        #endregion
    }
}