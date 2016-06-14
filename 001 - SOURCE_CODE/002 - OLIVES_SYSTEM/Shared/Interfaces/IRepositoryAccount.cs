using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4jClient;
using Shared.Models;
using Shared.Models.Nodes;
using Shared.ViewModels;

namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        #region Patient

        /// <summary>
        ///     Find patient by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<Patient>> FindPatientById(string id);

        /// <summary>
        ///     Filter patients by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePersonFilter> FilterPatientAsync(FilterPatientViewModel filter);

        /// <summary>
        ///     Initialize personal note of patient.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="note"></param>
        bool InitializePatientNote(string id, PersonalNote note);

        /// <summary>
        ///     Initialize an allergy connected to a person.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="allergy"></param>
        /// <returns></returns>
        bool InitializePatientAllergies(string id, Allergy allergy);

        /// <summary>
        ///     Initialize addiction causes to a patient by using patient id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="addiction"></param>
        /// <returns></returns>
        bool InitializePatientAddiction(string id, Addiction addiction);

        #endregion

        #region Doctor

        /// <summary>
        ///     Find doctor by using GUID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<Doctor>> FindDoctorById(string id);

        /// <summary>
        ///     Base on personal identity or identity card, check whether doctor can be registered or not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityCardNo"></param>
        /// <returns></returns>
        Task<bool> IsDoctorAbleToRegister(string id, string identityCardNo);

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="filter">Specific conditions</param>
        /// <returns></returns>
        Task<ResponsePersonFilter> FilterDoctorAsync(FilterDoctorViewModel filter);

        #endregion

        #region Shared

        /// <summary>
        ///     Initialize a person in database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Node<string>> InitializePerson(IPerson info);

        /// <summary>
        ///     Initialize a person attached with activation code.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<ResponsePersonCode> InitializePerson(IPerson info, ActivationCode code);

        /// <summary>
        ///     Update person information by using id for search.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IEnumerable<Node<string>>> EditPersonAsync(string id, IPerson info);

        /// <summary>
        ///     Using id and email to check whether person can be created or not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> IsPatientAbleToCreated(string id, string email);

        /// <summary>
        ///     Change account status base on account id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<bool> EditPersonStatus(string id, byte status);

        /// <summary>
        ///     Filter person by using specific conditions.
        /// </summary>
        /// <param name="filter">Specific conditions.</param>
        /// <returns></returns>
        Task<IEnumerable<Node<string>>> FilterPersonAsync(FilterPersonViewModel filter);

        /// <summary>
        ///     Log user in and retrieve the user information.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IPerson> LoginAsync(LoginViewModel info);

        /// <summary>
        ///     Retrieve personal information by using specific conditions.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        IPerson FindPerson(string email, string password, byte? role);

        /// <summary>
        ///     Find person by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Person> FindPerson(string id);

        /// <summary>
        ///     Statistic people by using role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<IList<StatusStatisticViewModel>> SummarizePersonRole(byte? role);

        #endregion
    }
}